namespace TODOList.Models
{
	public enum TimerType
	{
		Pomodoro,
		Countdown
	}

	public class TimerSession
	{
		public TimerType Type { get; set; } = TimerType.Pomodoro;
		public int DurationSeconds { get; set; } = 25 * 60;
		public int RemainingSeconds { get; set; } = 25 * 60;
		public bool IsRunning { get; set; }
		public bool IsBreak { get; set; }

		public string FormattedTime
		{
			get
			{
				var ts = TimeSpan.FromSeconds(RemainingSeconds);
				return ts.ToString(@"mm\:ss");
			}
		}

		public double Progress => DurationSeconds > 0
			? 1.0 - (double)RemainingSeconds / DurationSeconds
			: 0;
	}
}
