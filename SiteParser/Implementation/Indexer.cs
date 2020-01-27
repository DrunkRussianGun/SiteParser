using System;
using System.Threading.Tasks;
using SiteParser.Implementation.Database;
using SiteParser.Implementation.Indexing;
using SiteParser.Models;

namespace SiteParser.Implementation
{
	public class Indexer
	{
		private readonly Downloader _downloader;
		private readonly ElasticSearchClient _database;

		public Indexer(Downloader downloader, ElasticSearchClient database)
		{
			_downloader = downloader;
			_database = database;
		}

		public async Task<IndexingResult> IndexAsync(Uri pageUrl)
		{
			var page = await _downloader.DownloadPageAsync(pageUrl);
			
			var parsedHtml = HtmlParser.ParseHtml(page, pageUrl);
#pragma warning disable 4014
			_database.InsertAsync(new IndexedPage(pageUrl, parsedHtml.Text));
#pragma warning restore 4014
			
			return new IndexingResult(new[] {pageUrl});
		}
	}
}