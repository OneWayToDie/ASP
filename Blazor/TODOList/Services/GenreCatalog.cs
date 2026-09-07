namespace TODOList.Services
{
	public record GenreDef(string Key, string Display, int HitmoId, bool Approximate = false);

	public static class GenreCatalog
	{
		public static IReadOnlyList<GenreDef> All { get; } = new[]
		{
			new GenreDef("Alt-Metal_Shoegaze-Metal", "Alt-Metal", 199),
			new GenreDef("Black_Metal", "Black Metal", 197),
			new GenreDef("Death_Metal_Deathcore", "Death Metal", 196),
			new GenreDef("Doom_Metal", "Doom Metal", 198),
			new GenreDef("Hard_Rock_Heavy_Metal", "Hard Rock / Heavy Metal", 105),
			new GenreDef("indie", "Indie", 18),
			new GenreDef("Industrial_Metal_Electrocore", "Industrial Metal", 34),
			new GenreDef("Metalcore", "Metalcore", 192),
			new GenreDef("pop-rock", "Rock", 6),
			new GenreDef("Post-Hardcore", "Post-Hardcore", 100),
			new GenreDef("post-metal", "Post-Metal", 191),
			new GenreDef("post-rock", "Post-Rock", 80),
			new GenreDef("Power_Symphonic_Metal", "Power / Symphonic Metal", 10, Approximate: true),
			new GenreDef("Rapcore_Nu-metal", "Nu-metal", 93),
			new GenreDef("Sludge_Post-Metal", "Sludge", 190),
			new GenreDef("Thrash_Groove_Metal", "Thrash Metal", 195)
		};

		public static GenreDef? Find(string key)
		{
			foreach (var def in All)
			{
				if (def.Key == key) return def;
			}
			return null;
		}
	}
}