namespace TODOList.Models
{
	public enum Priority
	{
		Low,
		Medium,
		High
	}

	public class TodoItem
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool IsCompleted { get; set; }
		public Priority Priority { get; set; } = Priority.Medium;
		public string? Category { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? DueDate { get; set; }
	}
}
