using TODOList.Models;

namespace TODOList.Services
{
	public interface IRadioSource
	{
		string Name { get; }
		Task<IReadOnlyList<Track>?> LoadGenreAsync(GenreDef genre, CancellationToken ct = default);
	}
}