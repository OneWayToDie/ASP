using System.Text.RegularExpressions;
using TODOList.Models;

namespace TODOList.Services
{
	public class MusicService
	{
		private readonly IWebHostEnvironment _env;
		private readonly List<Track> _tracks = new();

		private static readonly string[] ImageExtensions = new[]
		{
			".jpg", ".jpeg", ".jfif", ".png", ".webp", ".avif", ".gif"
		};

		public IReadOnlyList<Track> Tracks => _tracks;

		private Track? _current;
		public Track? Current => _current;

		public bool IsPlaying { get; private set; }
		public bool RepeatOne { get; set; }
		public bool SyncWithTimer { get; set; } = true;

		private string? _currentGenre;
		public string? CurrentGenre
		{
			get => _currentGenre;
			set => _currentGenre = value;
		}

		private static readonly Dictionary<string, string> GenreDisplay = new(StringComparer.OrdinalIgnoreCase)
		{
			["Alt-Metal_Shoegaze-Metal"] = "Alt-Metal / Shoegaze",
			["Atmospheric_Black_Post-Black_Blackgaze"] = "Atmospheric Black / Post-Black / Blackgaze",
			["Black_Metal"] = "Black Metal",
			["breakcore"] = "Breakcore",
			["Death_Metal_Deathcore"] = "Death Metal / Deathcore",
			["Doom_Metal"] = "Doom Metal",
			["Emo-rock_Screamo"] = "Emo-rock / Screamo",
			["Hard_Rock_Heavy_Metal"] = "Hard Rock / Heavy Metal",
			["Industrial_Metal_Electrocore"] = "Industrial Metal / Electrocore",
			["Lo-fi_Chill-hop"] = "Lo-fi / Chill-hop",
			["Metalcore"] = "Metalcore",
			["pop-rock"] = "Pop-rock",
			["Post-Hardcore"] = "Post-Hardcore",
			["Power_Symphonic_Metal"] = "Power / Symphonic Metal",
			["Rapcore_Nu-metal"] = "Rapcore / Nu-metal",
			["Sludge_Post-Metal"] = "Sludge / Post-Metal",
			["Thrash_Groove_Metal"] = "Thrash / Groove Metal"
		};

		public string GetGenreDisplay(string genre)
		{
			if (GenreDisplay.TryGetValue(genre, out var name)) return name;
			return genre.Replace("_", " / ");
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

		public IEnumerable<string> Genres =>
			_tracks
				.Select(t => t.Genre)
				.Where(g => !string.IsNullOrWhiteSpace(g))
				.Select(g => g!)
				.Distinct()
				.OrderBy(g => g);

		private double _volume = 1.0;
		public double Volume
		{
			get => _volume;
			set { _volume = Math.Clamp(value, 0, 1); NotifyChange(); }
		}

		public event Action? OnChange;

		public MusicService(IWebHostEnvironment env)
		{
			_env = env;
			ScanMusic();
		}

		private void ScanMusic()
		{
			var musicDir = Path.Combine(_env.WebRootPath, "music");
			if (!Directory.Exists(musicDir)) return;

			var genreDirs = Directory.GetDirectories(musicDir).OrderBy(d => d).ToList();
			foreach (var genreDir in genreDirs)
			{
				var genre = Path.GetFileName(genreDir);

				var files = Directory.GetFiles(genreDir, "*.mp3")
					.OrderBy(f => Path.GetFileName(f))
					.ToList();

				foreach (var f in files)
				{
					var fileName = Path.GetFileName(f);
					var (title, artist) = ParseName(fileName);

					_tracks.Add(new Track
					{
						Title = title,
						Artist = artist,
						FileName = $"music/{Uri.EscapeDataString(genre)}/{Uri.EscapeDataString(fileName)}",
						Genre = genre
					});
				}
			}

			if (_tracks.Any() && _current == null)
			{
				_current = _tracks.First();
				_currentGenre = _current.Genre;
			}
		}

		private (string title, string artist) ParseName(string fileName)
		{
			var name = Path.GetFileNameWithoutExtension(fileName);

			// Remove common tags
			name = Regex.Replace(name, @"\s*\(www\.[^)]+\)", "", RegexOptions.IgnoreCase);
			name = Regex.Replace(name, @"\s*\[[^\]]*\]", "", RegexOptions.IgnoreCase);
			name = name.Trim();

			string artist = "Unknown";
			string title = name;

			// Normalize separators: "_-_" -> " - "
			name = name.Replace("_-_", " - ");

			var m = Regex.Match(name, @"^(.*?)\s*-\s*(.*?)$");
			if (m.Success && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
			{
				artist = m.Groups[1].Value.Trim();
				title = m.Groups[2].Value.Trim();
			}

			// Clean underscores inside the title and artist
			artist = artist.Replace('_', ' ').Replace("  ", " ").Trim();
			title = title.Replace('_', ' ').Replace("  ", " ").Trim();

			// Drop trailing numeric download ids
			title = Regex.Replace(title, @"[\s]+[0-9]+$", "").Trim();

			if (string.IsNullOrEmpty(artist)) artist = "Unknown";
			if (string.IsNullOrEmpty(title)) title = name;

			return (title, artist);
		}

		public IReadOnlyList<Track> GetTracks(string? genre)
		{
			if (string.IsNullOrEmpty(genre)) return _tracks;
			return _tracks.Where(t => t.Genre == genre).ToList();
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
			if (!pool.Any()) pool = GetTracks(null).ToList();
			if (!pool.Any()) return;

			var index = _current == null ? -1 : pool.FindIndex(t => t.Id == _current.Id);
			_current = pool[(index + 1) % pool.Count];
			_currentGenre = _current.Genre;
			NotifyChange();
		}

		public void PlayPrevious()
		{
			var genre = _current?.Genre;
			var pool = GetTracks(genre).ToList();
			if (!pool.Any()) pool = GetTracks(null).ToList();
			if (!pool.Any()) return;

			var index = _current == null ? 0 : pool.FindIndex(t => t.Id == _current.Id);
			var prev = pool[(index - 1 + pool.Count) % pool.Count];
			_current = prev;
			_currentGenre = _current.Genre;
			NotifyChange();
		}

		public void NotifyChange() => OnChange?.Invoke();
	}
}