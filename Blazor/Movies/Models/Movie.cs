using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
	public class Movie
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateOnly ReleaseDate { get; set; }
		public string Genre { get; set; }
	}
}
