using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteParser.Implementation;

namespace SiteParser.Controllers
{
	public class SearchController : ControllerBase
	{
		private readonly Searcher _searcher;

		public SearchController(Searcher searcher)
		{
			_searcher = searcher;
		}
		
		[HttpGet]
		public async Task<IActionResult> Search([FromQuery] Uri url)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!url.IsAbsoluteUri)
				return BadRequest($"URL \"{url}\" не является абсолютным.");

			var searchResult = await _searcher.SearchAsync(url);

			return Ok(searchResult);
		}
	}
}