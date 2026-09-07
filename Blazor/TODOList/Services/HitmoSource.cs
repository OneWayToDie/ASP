using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using TODOList.Models;

namespace TODOList.Services
{
	public static class RadioHttp
	{
		public static readonly HttpClient Shared = Create();

		private static HttpClient Create()
		{
			var handler = new SocketsHttpHandler
			{
				PooledConnectionLifetime = TimeSpan.FromMinutes(5),
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};
			var client = new HttpClient(handler)
			{
				Timeout = TimeSpan.FromSeconds(90)
			};
			client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
			client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,*/*;q=0.8");
			return client;
		}
	}

	public sealed class HitmoSource : IRadioSource
	{
		private const int PageSize = 48;
		private static readonly Regex MetaPattern = new(@"data-musmeta='([^']+)'", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private readonly string _baseUrl;
		private readonly TimeSpan _pageDelay;

		public string Name => "hitmo";
		public string BaseUrl => _baseUrl;

		public HitmoSource(string baseUrl, TimeSpan? pageDelay = null)
		{
			_baseUrl = baseUrl.TrimEnd('/');
			_pageDelay = pageDelay ?? TimeSpan.FromMilliseconds(700);
		}

		public async Task<GenreLoadResult?> LoadGenreAsync(GenreDef genre, int startPage, int pageCount, CancellationToken ct = default)
		{
			var tracks = new List<Track>();
			var pagesLoaded = 0;
			var exhausted = false;

			for (var page = startPage; page < startPage + pageCount && !ct.IsCancellationRequested; page++)
			{
				var pageUrl = page == 0
					? $"{_baseUrl}/genre/{genre.HitmoId}"
					: $"{_baseUrl}/genre/{genre.HitmoId}/start/{page * PageSize}";

				var batch = await FetchPageAsync(genre.Key, pageUrl, ct).ConfigureAwait(false);
				if (batch == null) return null;

				tracks.AddRange(batch);
				pagesLoaded++;
				if (batch.Count < PageSize)
				{
					exhausted = true;
					break;
				}

				await Task.Delay(_pageDelay, ct).ConfigureAwait(false);
			}

			return new GenreLoadResult { Tracks = tracks, PagesLoaded = pagesLoaded, Exhausted = exhausted };
		}

		private async Task<IReadOnlyList<Track>?> FetchPageAsync(string genreKey, string url, CancellationToken ct)
		{
			try
			{
				using var resp = await RadioHttp.Shared.GetAsync(url, ct).ConfigureAwait(false);
				if (resp.StatusCode == HttpStatusCode.NotFound) return Array.Empty<Track>();
				if (!resp.IsSuccessStatusCode)
				{
					try { Console.Error.WriteLine("hitmo fetch " + url + ": HTTP " + (int)resp.StatusCode + " (upstream unreachable?)"); } catch { }
					return null;
				}

				var html = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
				return ParsePage(genreKey, html);
			}
			catch (OperationCanceledException) { return null; }
			catch (Exception ex)
			{
				try { Console.Error.WriteLine("hitmo fetch " + url + ": " + ex.Message); } catch { }
				return null;
			}
		}

		private IReadOnlyList<Track> ParsePage(string genreKey, string html)
		{
			var list = new List<Track>();
			foreach (Match m in MetaPattern.Matches(html))
			{
				try
				{
					using var doc = JsonDocument.Parse(m.Groups[1].Value);
					var root = doc.RootElement;
					var title = Get(root, "title");
					if (string.IsNullOrWhiteSpace(title)) continue;

					var url = Get(root, "url");
					if (string.IsNullOrWhiteSpace(url)) continue;

					var radioId = ParseId(Get(root, "id"), url);
					if (radioId <= 0) continue;

					var artist = Get(root, "artist");
					var img = Get(root, "img");
					var streamUrl = url.Contains("://") ? url : _baseUrl + "/" + url.TrimStart('/');

					list.Add(new Track
					{
						Title = title,
						Artist = string.IsNullOrWhiteSpace(artist) ? "Unknown" : artist,
						Genre = genreKey,
						RadioId = radioId,
						CoverUrl = string.IsNullOrWhiteSpace(img) ? null : img,
						StreamUrl = streamUrl,
						FileName = BuildPlayPath(genreKey, radioId)
					});
				}
				catch (JsonException) { }
				catch { }
			}
			return list;
		}

		internal static string BuildPlayPath(string genreKey, long radioId)
			=> $"api/radio/{Uri.EscapeDataString(genreKey)}/{radioId}";

		private static string Get(JsonElement root, string name)
			=> root.TryGetProperty(name, out var el) && el.ValueKind == JsonValueKind.String
				? el.GetString() ?? string.Empty
				: string.Empty;

		private static long ParseId(string idRaw, string url)
		{
			if (!string.IsNullOrEmpty(idRaw))
			{
				var last = idRaw.Split('-')[^1];
				if (long.TryParse(last, out var v)) return v;
			}
			if (!string.IsNullOrEmpty(url))
			{
				var segs = url.TrimEnd('/').Split('/');
				if (segs.Length >= 2 && long.TryParse(segs[^2], out var v2)) return v2;
			}
			return 0;
		}
	}
}