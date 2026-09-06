namespace TODOList.Services
{
	public record GenreDef(string Key, string Display, int HitmoId, bool Approximate = false);

	public static class GenreCatalog
	{
		public static IReadOnlyList<GenreDef> All { get; } = new[]
		{
			new GenreDef("Alt-Metal_Shoegaze-Metal", "Alt-Metal / Shoegaze", 199),
			new GenreDef("Atmospheric_Black_Post-Black_Blackgaze", "Atmospheric Black / Post-Black / Blackgaze", 197, Approximate: true),
			new GenreDef("Black_Metal", "Black Metal", 197),
			new GenreDef("breakcore", "Breakcore", 193, Approximate: true),
			new GenreDef("Death_Metal_Deathcore", "Death Metal / Deathcore", 196),
			new GenreDef("Doom_Metal", "Doom Metal", 198),
			new GenreDef("Emo-rock_Screamo", "Emo-rock / Screamo", 213, Approximate: true),
			new GenreDef("Hard_Rock_Heavy_Metal", "Hard Rock / Heavy Metal", 105),
			new GenreDef("Industrial_Metal_Electrocore", "Industrial Metal / Electrocore", 34),
			new GenreDef("Lo-fi_Chill-hop", "Lo-fi / Chill-hop", 194, Approximate: true),
			new GenreDef("Metalcore", "Metalcore", 192),
			new GenreDef("pop-rock", "Pop-rock", 6),
			new GenreDef("Post-Hardcore", "Post-Hardcore", 100),
			new GenreDef("Power_Symphonic_Metal", "Power / Symphonic Metal", 10, Approximate: true),
			new GenreDef("Rapcore_Nu-metal", "Rapcore / Nu-metal", 93),
			new GenreDef("Sludge_Post-Metal", "Sludge / Post-Metal", 190),
			new GenreDef("Thrash_Groove_Metal", "Thrash / Groove Metal", 195)
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