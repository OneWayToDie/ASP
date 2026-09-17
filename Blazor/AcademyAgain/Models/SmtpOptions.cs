namespace AcademyAgain.Models
{
	public class SmtpOptions
	{
		public string Host { get; set; } = string.Empty;
		public int Port { get; set; } = 465;
		public bool UseSsl { get; set; } = true;
		public string User { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string From { get; set; } = string.Empty;
		public string FromName { get; set; } = "Academy";

		public bool IsConfigured =>
			!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(From);
	}
}
