using System.Net.Http.Headers;

namespace TODOList.Services
{
	/// <summary>
	/// Transparent audio proxy for radio streams. The app streams every track
	/// through its own origin (/api/radio/...) so the browser always sees
	/// same-origin media: no CORS needed, the spectrum visualizer
	/// (Web Audio AnalyserNode) works, and Range/206 passthrough keeps seeking
	/// fast. Nothing is stored server-side.
	/// </summary>
	public static class RadioProxy
	{
		public const string RoutePrefix = "/api/radio/";

		public static Task HandleAsync(HttpContext context, MusicService music, string remainingPath)
		{
			var parts = remainingPath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 2 || !long.TryParse(parts[1], out var radioId))
				return WriteAsync(context, 400, "bad request");

			var genreKey = Uri.UnescapeDataString(parts[0]);
			var track = music.FindRadioTrack(genreKey, radioId);
			if (track == null || string.IsNullOrWhiteSpace(track.StreamUrl))
				return WriteAsync(context, 404, "track not found");

			return StreamAsync(context, track.StreamUrl, context.RequestAborted);
		}

		private static async Task StreamAsync(HttpContext context, string upstreamUrl, CancellationToken ct)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, upstreamUrl);
			var rangeHeader = context.Request.Headers.Range.ToString();
			if (!string.IsNullOrWhiteSpace(rangeHeader) && RangeHeaderValue.TryParse(rangeHeader, out var range))
			{
				request.Headers.Range = range;
			}

			HttpResponseMessage upstream;
			try
			{
				upstream = await RadioHttp.Shared.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
			}
			catch (OperationCanceledException) { return; }
			catch (Exception)
			{
				await WriteAsync(context, 502, "upstream error");
				return;
			}

			using var response = upstream;
			var status = (int)response.StatusCode;
			if (status < 200 || status >= 300)
			{
				await WriteAsync(context, status == 404 ? 404 : 502, "upstream error");
				return;
			}

			context.Response.StatusCode = status;

			var contentType = response.Content.Headers.ContentType?.ToString();
			if (!string.IsNullOrEmpty(contentType)) context.Response.ContentType = contentType;

			if (response.Content.Headers.ContentRange is not null)
				context.Response.Headers["Content-Range"] = response.Content.Headers.ContentRange.ToString();
			if (response.Headers.TryGetValues("Accept-Ranges", out var arValues))
				context.Response.Headers["Accept-Ranges"] = arValues.FirstOrDefault()!;
			if (response.Content.Headers.ContentLength is long length)
				context.Response.ContentLength = length;

			context.Response.Headers["Access-Control-Allow-Origin"] = "*";

			await using var body = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
			await body.CopyToAsync(context.Response.Body, 81920, ct).ConfigureAwait(false);
		}

		private static Task WriteAsync(HttpContext context, int status, string message)
		{
			context.Response.StatusCode = status;
			context.Response.ContentType = "text/plain; charset=utf-8";
			return context.Response.WriteAsync(message);
		}
	}
}