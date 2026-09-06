namespace TODOList.Models
{
	public class Track
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string Artist { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public TimeSpan Duration { get; set; }
		public string? Genre { get; set; }
		public long RadioId { get; set; }
		public string? CoverUrl { get; set; }
		public string? StreamUrl { get; set; }
	}
}