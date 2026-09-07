using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using TODOList.Models;

namespace TODOList.Services
{
	public class MusicService : IDisposable
	{
		private const int InitialPages = 4;

		private readonly IWebHostEnvironment _env;
		private readonly IRadioSource _source;
		private readonly NotificationService _notifications;

		private readonly ConcurrentDictionary<string, IReadOnlyList<Track>> _genreTracks = new();
		private readonly Dictionary<string, int> _pagesLoaded = new();
		private readonly HashSet<string> _completed = new();
		private readonly object _sync = new();
		private readonly object _ioLock = new();
		private readonly SemaphoreSlim _crawlLock = new(1, 1);
		private readonly string _cachePath;

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

		public MusicService(IWebHostEnvironment env, IRadioSource source, NotificationService notifications)
		{
			_env = env;
			_source = source;
			_notifications = notifications;
			_cachePath = Path.Combine(env.ContentRootPath, "data", "radio-cache.json");
			LoadCache();
			SaveCache();
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

		/// <summary>
		/// One incremental crawl pass: for every genre not yet marked complete,
		/// load the next up-to-<paramref name="pagesPerGenre"/> pages and append tracks.
		/// Genres that return an empty/paged-out response are marked complete and excluded forever.
		/// </summary>
		public async Task<string> RunCrawlPassAsync(int pagesPerGenre = 3, CancellationToken ct = default)
		{
			var report = new StringBuilder();
			if (pagesPerGenre < 1) pagesPerGenre = 1;

			if (!await _crawlLock.WaitAsync(0).ConfigureAwait(false))
			{
				report.AppendLine("уже идёт проход докачки — подожди и повтори позже");
				return report.ToString();
			}

			try
			{
				foreach (var def in GenreCatalog.All)
				{
					if (ct.IsCancellationRequested) break;

					bool done;
					lock (_sync) done = _completed.Contains(def.Key);
					if (done)
					{
						report.AppendLine($"• {def.Key}: завершён, пропущен");
						continue;
					}

					int startPage;
					lock (_sync) startPage = _pagesLoaded.TryGetValue(def.Key, out var sp) ? sp : 0;

					var result = await _source.LoadGenreAsync(def, startPage, pagesPerGenre, ct).ConfigureAwait(false);
					if (result == null)
					{
						report.AppendLine($"! {def.Key}: HTTP/сетевой сбой — повторю в следующем проходе");
						continue;
					}

					MergeTracks(def.Key, result.Tracks);

					int pagesNow, total;
					lock (_sync)
					{
						_pagesLoaded[def.Key] = startPage + result.PagesLoaded;
						if (result.Exhausted) _completed.Add(def.Key);
						pagesNow = _pagesLoaded[def.Key];
						total = _genreTracks.TryGetValue(def.Key, out var list) ? list.Count : 0;
					}
					SaveCache();

					if (result.Exhausted)
					{
						report.AppendLine($"✓ {def.Key}: исчерпан на странице {pagesNow} ({total} треков), исключён из докачки");
						_notifications.Push("Жанр исчерпан", $"{GetGenreDisplay(def.Key)} — {total} треков", false);
					}
					else
					{
						report.AppendLine($"+ {def.Key}: загружено страниц {result.PagesLoaded} (всего {pagesNow}), треков {total}");
					}

					if (_current == null && total > 0)
					{
						lock (_sync)
						{
							if (_current == null && _genreTracks.TryGetValue(def.Key, out var list) && list.Count > 0)
							{
								_current = list[0];
								_currentGenre = def.Key;
							}
						}
					}
					NotifyChange();
				}
			}
			finally
			{
				_crawlLock.Release();
			}

			return report.ToString();
		}

		public string GetStatusReport()
		{
			var sb = new StringBuilder();
			foreach (var def in GenreCatalog.All)
			{
				int pages, total;
				bool done;
				lock (_sync)
				{
					pages = _pagesLoaded.TryGetValue(def.Key, out var p) ? p : 0;
					total = _genreTracks.TryGetValue(def.Key, out var list) ? list.Count : 0;
					done = _completed.Contains(def.Key);
				}
				sb.AppendLine($"{def.Key,-36} стр.{pages,-3} треков:{total,-5} {(done ? "ЗАВЕРШЁН" : "")}");
			}
			return sb.ToString();
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

		private async Task RefreshGenreAsync(GenreDef def, CancellationToken ct = default)
		{
			try
			{
				var result = await _source.LoadGenreAsync(def, 0, InitialPages, ct).ConfigureAwait(false);
				if (result == null) return;

				foreach (var t in result.Tracks)
				{
					t.Genre = def.Key;
					t.FileName = HitmoSource.BuildPlayPath(def.Key, t.RadioId);
				}
				_genreTracks[def.Key] = result.Tracks;

				lock (_sync)
				{
					_pagesLoaded[def.Key] = result.PagesLoaded;
					if (result.Exhausted) _completed.Add(def.Key);
				}
				SaveCache();

				if (_current == null && result.Tracks.Count > 0)
				{
					_current = result.Tracks[0];
					_currentGenre = def.Key;
				}
				NotifyChange();
			}
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				try { Console.Error.WriteLine("radio load " + def.Key + ": " + ex.Message); } catch { }
			}
		}

		private void MergeTracks(string genreKey, IReadOnlyList<Track> incoming)
		{
			var existing = _genreTracks.TryGetValue(genreKey, out var prev) ? prev.ToList() : new List<Track>();
			var seen = new HashSet<long>(existing.Select(t => t.RadioId));
			foreach (var t in incoming)
			{
				if (!seen.Add(t.RadioId)) continue;
				t.Genre = genreKey;
				t.FileName = HitmoSource.BuildPlayPath(genreKey, t.RadioId);
				existing.Add(t);
			}
			_genreTracks[genreKey] = existing;
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
					if (GenreCatalog.Find(group.Key) == null) continue;

					foreach (var t in group.Tracks)
					{
						t.Genre = group.Key;
						t.FileName = HitmoSource.BuildPlayPath(group.Key, t.RadioId);
					}
					_genreTracks[group.Key] = group.Tracks;

					lock (_sync)
					{
						_pagesLoaded[group.Key] = group.LoadedPages > 0 ? group.LoadedPages : InitialPages;
						if (group.Completed) _completed.Add(group.Key);
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
							if (GenreCatalog.Find(kv.Key) == null) continue;
							groups.Add(new CacheGenre
							{
								Key = kv.Key,
								RefreshedUtc = DateTime.UtcNow.ToString("O"),
								Tracks = kv.Value.ToList(),
								LoadedPages = _pagesLoaded.TryGetValue(kv.Key, out var p) ? p : InitialPages,
								Completed = _completed.Contains(kv.Key)
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
			public int LoadedPages { get; set; }
			public bool Completed { get; set; }
		}

		public void Dispose()
		{
			try { _crawlLock.Dispose(); } catch { }
		}
	}
}