namespace TODOList
{
	public class Task
	{
		public string Description { get; set; }
		public bool DONE { get; set; }
		public DateTime AssignTime { get; set; }

		public override bool Equals(object? other)
		{
			return this.Description.Equals
				(
					(other as TODOList.Task).Description,
					StringComparison.OrdinalIgnoreCase
				);
		}
		//public override string ToString()
		//{
		//	return $"<input type=\"checkbox\" @bind=\"{DONE}\" /><span>{Description}</span><span>@task.AssignTime</span>";
		//}
	}
}
