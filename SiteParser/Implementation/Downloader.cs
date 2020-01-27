using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SiteParser.Implementation
{
	public class Downloader
	{
		private readonly HttpClient _httpClient;

		public Downloader(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("PageDownloader");
		}

		public async Task<string> DownloadPageAsync(Uri url)
		{
			try
			{
				using var response = await _httpClient.GetAsync(url);
				var content = await response.Content.ReadAsStringAsync();
				return content;
			}
			catch
			{
				return null;
			}
		}
	}
}