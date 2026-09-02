namespace TODOList.Models
{
	public class Reminder
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string? Message { get; set; }
		public DateTime TriggerAt { get; set; }
		public bool IsFired { get; set; }
		public Guid? LinkedTaskId { get; set; }
	}
}
