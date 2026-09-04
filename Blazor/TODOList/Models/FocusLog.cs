namespace TODOList.Models
{
	public enum LogKind
	{
		Pomodoro,
		TaskDone
	}

	public class FocusLog
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public DateTime Timestamp { get; set; } = DateTime.Now;
		public LogKind Kind { get; set; }
		public int Seconds { get; set; }
		public string? TaskId { get; set; }
	}
}
