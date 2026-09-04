using System.Text.RegularExpressions;
using TODOList.Models;

namespace TODOList.Services
{
	public class MusicService
	{
		private readonly IWebHostEnvironment _env;
		private readonly List<Track> _tracks = new();

		public IReadOnlyList<Track> Tracks => _tracks;

		private Track? _current;
		public Track? Current => _current;

		public bool IsPlaying { get; private set; }
		public bool RepeatOne { get; set; }
		public bool SyncWithTimer { get; set; } = true;
		public PlaylistState CurrentPlaylist { get; set; } = PlaylistState.Focus;

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

			var files = Directory.GetFiles(musicDir, "*.mp3")
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
					FileName = $"music/{Uri.EscapeDataString(fileName)}",
					Playlist = AssignPlaylist(title, artist)
				});
			}

			if (_tracks.Any() && _current == null)
			{
				_current = _tracks.First();
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

		private PlaylistState AssignPlaylist(string title, string artist)
		{
			var text = (title + " " + artist).ToLowerInvariant();

			// Keyed by content for the depressive/break vibes
			if (text.Contains("numb") || text.Contains("suicide") || text.Contains("raya")
				|| text.Contains("liar") || text.Contains("dying") || text.Contains("dark"))
				return PlaylistState.Depressed;

			if (text.Contains("rasstrel") || text.Contains("cut") || text.Contains("moon")
				|| text.Contains("paranormal") || text.Contains("cloth") || text.Contains("omens"))
				return PlaylistState.Focus;

			return PlaylistState.Break;
		}

		public IReadOnlyList<Track> GetTracks(PlaylistState state)
		{
			if (state == PlaylistState.All) return _tracks;
			return _tracks.Where(t => t.Playlist == state).ToList();
		}

		public void SetCurrent(Track track)
		{
			_current = track;
			CurrentPlaylist = track.Playlist;
			NotifyChange();
		}

		public bool EnsurePlaylist(PlaylistState state)
		{
			if (CurrentPlaylist == state && _current != null) return false;

			var pool = GetTracks(state);
			if (!pool.Any())
			{
				pool = GetTracks(PlaylistState.All);
				if (!pool.Any()) return false;
				state = PlaylistState.All;
			}

			_current = pool.First();
			CurrentPlaylist = state;
			NotifyChange();
			return true;
		}

		public void SetPlaying(bool playing)
		{
			IsPlaying = playing;
			NotifyChange();
		}

		public void PlayNext()
		{
			var pool = GetTracks(CurrentPlaylist);
			if (!pool.Any())
			{
				pool = GetTracks(PlaylistState.All);
				if (!pool.Any()) return;
			}

			var index = _current == null ? -1 : pool.ToList().FindIndex(t => t.Id == _current.Id);
			var next = pool[(index + 1) % pool.Count];
			_current = next;
			NotifyChange();
		}

		public void PlayPrevious()
		{
			var pool = GetTracks(CurrentPlaylist);
			if (!pool.Any())
			{
				pool = GetTracks(PlaylistState.All);
				if (!pool.Any()) return;
			}

			var index = _current == null ? 0 : pool.ToList().FindIndex(t => t.Id == _current.Id);
			var prev = pool[(index - 1 + pool.Count) % pool.Count];
			_current = prev;
			NotifyChange();
		}

		public void NotifyChange() => OnChange?.Invoke();
	}
}
