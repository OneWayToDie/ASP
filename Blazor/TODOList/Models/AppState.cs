namespace TODOList.Models
{
	public class AppState
	{
		public List<TodoItem> Tasks { get; set; } = new();
		public List<Reminder> Reminders { get; set; } = new();
		public List<Category> Categories { get; set; } = new();
		public List<FocusLog> Logs { get; set; } = new();
		public bool TimerPomodoro { get; set; } = true;
		public int TimerDurationSeconds { get; set; } = 25 * 60;
		public string? Genre { get; set; }
	}
}
