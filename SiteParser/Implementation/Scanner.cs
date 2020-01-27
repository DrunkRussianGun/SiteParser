using System;
using System.Threading.Tasks;
using SiteParser.Implementation.Database;
using SiteParser.Implementation.Indexing;
using SiteParser.Models;

namespace SiteParser.Implementation
{
	public class Scanner
	{
		private readonly Downloader _downloader;
		private readonly ElasticSearchClient _database;

		public Scanner(Downloader downloader, ElasticSearchClient database)
		{
			_downloader = downloader;
			_database = database;
		}

		public async Task<ScanResult> ScanAsync(Uri pageUrl)
		{
			var page = await _downloader.DownloadPageAsync(pageUrl);
			
			var parsedHtml = HtmlParser.ParseHtml(page, pageUrl);
#pragma warning disable 4014
			_database.InsertAsync(new ScannedPage(pageUrl, parsedHtml.Text));
#pragma warning restore 4014
			
			return new ScanResult(new[] {pageUrl});
		}
	}
}