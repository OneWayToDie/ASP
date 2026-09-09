using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
	public class Movie
	{
		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		[RangeAttribute(typeof(DateOnly), "1895-12-28", "9999-12-31")]
		public string Title { get; set; }
		public DateOnly ReleaseDate { get; set; }
		public string Genre { get; set; }
		public string? URL { get; set; }
		public string? Poster { get; set; }
	}
}
