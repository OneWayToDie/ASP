using System.Collections.Concurrent;
using System.Text.Json;
using TODOList.Models;

namespace TODOList.Services
{
	public class MusicService : IDisposable
	{
		private readonly IWebHostEnvironment _env;
		private readonly IRadioSource _source;

		private readonly ConcurrentDictionary<string, IReadOnlyList<Track>> _genreTracks = new();
		private readonly Dictionary<string, DateTime> _lastRefresh = new();
		private readonly object _sync = new();
		private readonly object _ioLock = new();
		private readonly CancellationTokenSource _cts = new();
		private readonly string _cachePath;

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(6);
		private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

		private static readonly string[] ImageExtensions = new[]
		{
			".jpg", ".jpeg", ".jfif", ".png", ".webp", ".avif", ".gif"
		};

		private Track? _current;
		public Track? Current => _current;

		public bool IsPlaying { get; private set; }
		public bool RepeatOne { get; set; }
		public bool SyncWithTimer { get; set; } = true;

		private string _genrePickerVariant = "pills";
		public string GenrePickerVariant
		{
			get => _genrePickerVariant;
			set
			{
				if (_genrePickerVariant == value) return;
				_genrePickerVariant = value;
				NotifyChange();
			}
		}

		private string _playerTheme = "wallpaper";
		public string PlayerTheme
		{
			get => _playerTheme;
			set
			{
				if (_playerTheme == value) return;
				_playerTheme = value;
				NotifyChange();
			}
		}

		private string? _currentGenre;
		public string? CurrentGenre
		{
			get => _currentGenre;
			set => _currentGenre = value;
		}

		public IEnumerable<string> Genres => GenreCatalog.All.Select(g => g.Key);

		public string GetGenreDisplay(string genre)
		{
			var def = GenreCatalog.Find(genre);
			return def?.Display ?? genre.Replace("_", " / ");
		}

		public string GetGenreAccent(string genre)
		{
			var hash = 0;
			foreach (var c in genre)
			{
				hash = (hash * 31 + c) & 0x7fffffff;
			}
			var hue = (hash % 360 + 360) % 360;
			return $"hsl({hue} 65% 55%)";
		}

		private double _volume = 1.0;
		public double Volume
		{
			get => _volume;
			set { _volume = Math.Clamp(value, 0, 1); NotifyChange(); }
		}

		public event Action? OnChange;

		public MusicService(IWebHostEnvironment env, IRadioSource source)
		{
			_env = env;
			_source = source;
			_cachePath = Path.Combine(env.ContentRootPath, "data", "radio-cache.json");
			LoadCache();
			_ = Task.Run(() => RunRefreshLoopAsync(_cts.Token));
		}

		public IReadOnlyList<Track> Tracks => GetTracks(null);

		public IReadOnlyList<Track> GetTracks(string? genre)
		{
			if (string.IsNullOrEmpty(genre))
			{
				return _genreTracks.Values.SelectMany(l => l).ToList();
			}
			return _genreTracks.TryGetValue(genre, out var list) ? list : Array.Empty<Track>();
		}

		public Track? FindRadioTrack(string genreKey, long radioId)
		{
			if (!_genreTracks.TryGetValue(genreKey, out var list)) return null;
			foreach (var t in list)
			{
				if (t.RadioId == radioId) return t;
			}
			return null;
		}

		public IReadOnlyList<string> GetWallpapers(string genre)
		{
			try
			{
				var wallDir = Path.Combine(_env.WebRootPath, "wallpapers", genre);
				if (!Directory.Exists(wallDir)) return Array.Empty<string>();

				return Directory.GetFiles(wallDir)
					.Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
					.OrderBy(f => Path.GetFileName(f))
					.Select(f => $"wallpapers/{Uri.EscapeDataString(genre)}/{Uri.EscapeDataString(Path.GetFileName(f))}")
					.ToList();
			}
			catch
			{
				return Array.Empty<string>();
			}
		}

		public IReadOnlyList<string> GetBackgrounds(string genre)
		{
			try
			{
				var wallDir = Path.Combine(_env.WebRootPath, "wallpapers", genre);
				if (!Directory.Exists(wallDir)) return Array.Empty<string>();

				var hdDir = Path.Combine(_env.WebRootPath, "wallpapers2560", genre);
				var hasHd = Directory.Exists(hdDir);

				return Directory.GetFiles(wallDir)
					.Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
					.OrderBy(f => Path.GetFileName(f))
					.Select(f =>
					{
						var fileName = Path.GetFileName(f);
						if (hasHd)
						{
							var hdFile = Path.Combine(hdDir, Path.GetFileNameWithoutExtension(fileName) + ".jpg");
							if (File.Exists(hdFile))
							{
								return $"wallpapers2560/{Uri.EscapeDataString(genre)}/{Uri.EscapeDataString(Path.GetFileName(hdFile))}";
							}
						}
						return $"wallpapers/{Uri.EscapeDataString(genre)}/{Uri.EscapeDataString(fileName)}";
					})
					.ToList();
			}
			catch
			{
				return Array.Empty<string>();
			}
		}

		public async Task EnsureGenreTracksAsync(string? genre)
		{
			if (string.IsNullOrEmpty(genre)) return;
			var def = GenreCatalog.Find(genre);
			if (def == null) return;
			if (_genreTracks.ContainsKey(def.Key)) return;
			await RefreshGenreAsync(def).ConfigureAwait(false);
		}

		public void SetCurrent(Track track)
		{
			_current = track;
			_currentGenre = track.Genre;
			NotifyChange();
		}

		public void SetPlaying(bool playing)
		{
			IsPlaying = playing;
			NotifyChange();
		}

		public void PlayNext()
		{
			var genre = _current?.Genre;
			var pool = GetTracks(genre).ToList();
			if (pool.Count == 0) pool = GetTracks(null).ToList();
			if (pool.Count == 0) return;

			var index = _current == null ? -1 : pool.FindIndex(t => t.Id == _current.Id);
			_current = pool[(index + 1) % pool.Count];
			_currentGenre = _current.Genre;
			NotifyChange();
		}

		public void PlayPrevious()
		{
			var genre = _current?.Genre;
			var pool = GetTracks(genre).ToList();
			if (pool.Count == 0) pool = GetTracks(null).ToList();
			if (pool.Count == 0) return;

			var index = _current == null ? 0 : pool.FindIndex(t => t.Id == _current.Id);
			_current = pool[(index - 1 + pool.Count) % pool.Count];
			_currentGenre = _current.Genre;
			NotifyChange();
		}

		public void NotifyChange() => OnChange?.Invoke();

		private bool NeedsRefresh(GenreDef def)
		{
			lock (_sync)
			{
				return !_lastRefresh.TryGetValue(def.Key, out var dt)
					|| DateTime.UtcNow - dt > RefreshInterval;
			}
		}

		private async Task RunRefreshLoopAsync(CancellationToken ct)
		{
			try
			{
				foreach (var def in GenreCatalog.All)
				{
					if (ct.IsCancellationRequested) return;
					if (NeedsRefresh(def)) await RefreshGenreAsync(def, ct).ConfigureAwait(false);
					await Task.Delay(1500, ct).ConfigureAwait(false);
				}

				while (!ct.IsCancellationRequested)
				{
					await Task.Delay(RefreshInterval, ct).ConfigureAwait(false);
					foreach (var def in GenreCatalog.All)
					{
						if (ct.IsCancellationRequested) return;
						if (NeedsRefresh(def)) await RefreshGenreAsync(def, ct).ConfigureAwait(false);
						await Task.Delay(1500, ct).ConfigureAwait(false);
					}
				}
			}
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				try { Console.Error.WriteLine("radio refresh loop: " + ex); } catch { }
			}
		}

		private async Task RefreshGenreAsync(GenreDef def, CancellationToken ct = default)
		{
			IReadOnlyList<Track>? tracks;
			try
			{
				tracks = await _source.LoadGenreAsync(def, ct).ConfigureAwait(false);
			}
			catch (OperationCanceledException) { return; }
			catch (Exception ex)
			{
				try { Console.Error.WriteLine("radio load " + def.Key + ": " + ex.Message); } catch { }
				return;
			}

			if (tracks == null || tracks.Count == 0) return;

			_genreTracks[def.Key] = tracks;
			lock (_sync) { _lastRefresh[def.Key] = DateTime.UtcNow; }
			SaveCache();

			if (_current == null)
			{
				_current = tracks[0];
				_currentGenre = def.Key;
			}
			NotifyChange();
		}

		private void LoadCache()
		{
			try
			{
				if (!File.Exists(_cachePath)) return;
				var json = File.ReadAllText(_cachePath);
				var dto = JsonSerializer.Deserialize<CacheDto>(json, JsonOptions);
				if (dto?.Genres == null) return;

				foreach (var group in dto.Genres)
				{
					if (group == null || string.IsNullOrEmpty(group.Key) || group.Tracks == null) continue;
					if (group.Tracks.Count == 0) continue;

					foreach (var t in group.Tracks)
					{
						t.Genre = group.Key;
						t.FileName = HitmoSource.BuildPlayPath(group.Key, t.RadioId);
					}
					_genreTracks[group.Key] = group.Tracks;

					if (DateTime.TryParse(group.RefreshedUtc, out var dt))
					{
						lock (_sync) { _lastRefresh[group.Key] = dt; }
					}
				}
			}
			catch (Exception ex)
			{
				try { Console.Error.WriteLine("radio cache load: " + ex.Message); } catch { }
			}
		}

		private void SaveCache()
		{
			lock (_ioLock)
			{
				try
				{
					var groups = new List<CacheGenre>();
					lock (_sync)
					{
						foreach (var kv in _genreTracks)
						{
							_lastRefresh.TryGetValue(kv.Key, out var dt);
							groups.Add(new CacheGenre
							{
								Key = kv.Key,
								RefreshedUtc = dt.ToString("O"),
								Tracks = kv.Value.ToList()
							});
						}
					}

					var dir = Path.GetDirectoryName(_cachePath);
					if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

					var tmp = _cachePath + ".tmp";
					File.WriteAllText(tmp, JsonSerializer.Serialize(new CacheDto { Genres = groups }, JsonOptions));
					File.Move(tmp, _cachePath, true);
				}
				catch (Exception ex)
				{
					try { Console.Error.WriteLine("radio cache save: " + ex.Message); } catch { }
				}
			}
		}

		private sealed class CacheDto
		{
			public List<CacheGenre>? Genres { get; set; }
		}

		private sealed class CacheGenre
		{
			public string Key { get; set; } = string.Empty;
			public string RefreshedUtc { get; set; } = string.Empty;
			public List<Track>? Tracks { get; set; }
		}

		public void Dispose()
		{
			try { _cts.Cancel(); _cts.Dispose(); } catch { }
		}
	}
}