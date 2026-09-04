namespace TODOList.Models
{
	public class Category
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Name { get; set; } = string.Empty;
		public string Color { get; set; } = "#ffffff";
		public string Icon { get; set; } = "bi-tag";
		public bool IsDefault { get; set; }
	}
}
