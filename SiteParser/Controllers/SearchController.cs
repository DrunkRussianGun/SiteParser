using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteParser.Implementation;
using SiteParser.Models;

namespace SiteParser.Controllers
{
	public class SearchController : ControllerBase
	{
		private readonly Searcher _searcher;

		public SearchController(Searcher searcher)
		{
			_searcher = searcher;
		}
		
		[HttpPost]
		public async Task<ActionResult<IndexedPage[]>> Search([FromBody] Uri domainUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!domainUrl.IsAbsoluteUri)
				return BadRequest($"URL \"{domainUrl}\" не является абсолютным.");

			var searchResult = await _searcher.SearchAsync(domainUrl);

			return Ok(searchResult);
		}
	}
}