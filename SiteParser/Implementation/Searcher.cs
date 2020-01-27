using System;
using System.Threading.Tasks;
using SiteParser.Implementation.Database;
using SiteParser.Models;

namespace SiteParser.Implementation
{
	public class Searcher
	{
		private readonly ElasticSearchClient _database;

		public Searcher(ElasticSearchClient database)
		{
			_database = database;
		}
		
		public async Task<IndexedPage[]> SearchAsync(Uri domainUrl)
		{
			var pages = await _database.FindAsync(domainUrl.ToString());

			return pages;
		}
	}
}