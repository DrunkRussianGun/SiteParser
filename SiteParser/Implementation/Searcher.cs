using System;
using System.Linq;
using System.Threading.Tasks;
using SiteParser.Implementation.Database;
using SiteParser.Implementation.Scan;
using SiteParser.Models;

namespace SiteParser.Implementation
{
	public class Searcher
	{
		private readonly ElasticSearchClient _database;
		private readonly UrlFilter _urlFilter;

		public Searcher(ElasticSearchClient database, UrlFilter urlFilter = null)
		{
			_database = database;
			_urlFilter = urlFilter ?? new UrlFilter();
		}
		
		public async Task<ScannedPage[]> SearchAsync(Uri domainUrl)
		{
			var pages = await _database.GetAllAsync();

			var urls = pages.Select(page => page.Url);
			var filteredUrls = _urlFilter.Filter(urls, domainUrl);

			return pages
				.Where(page => filteredUrls.Contains(page.Url))
				.ToArray();
		}
	}
}