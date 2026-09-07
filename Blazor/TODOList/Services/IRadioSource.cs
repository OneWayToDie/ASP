using TODOList.Models;

namespace TODOList.Services
{
	public class GenreLoadResult
	{
		public IReadOnlyList<Track> Tracks { get; set; } = Array.Empty<Track>();
		public int PagesLoaded { get; set; }
		public bool Exhausted { get; set; }
	}

	public interface IRadioSource
	{
		string Name { get; }
		Task<GenreLoadResult?> LoadGenreAsync(GenreDef genre, int startPage, int pageCount, CancellationToken ct = default);
	}
}