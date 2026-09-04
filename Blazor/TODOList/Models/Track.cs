namespace TODOList.Models
{
	public enum PlaylistState
	{
		All,
		Focus,
		Break,
		Depressed
	}

	public class Track
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string Artist { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public TimeSpan Duration { get; set; }
		public PlaylistState Playlist { get; set; } = PlaylistState.All;
	}
}
