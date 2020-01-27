using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteParser.Implementation;

namespace SiteParser.Controllers
{
	public class IndexController : ControllerBase
	{
		private readonly Indexer _indexer;

		public IndexController(Indexer indexer)
		{
			_indexer = indexer;
		}
		
		[HttpPost]
		public async Task<IActionResult> Index([FromBody] Uri pageUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!pageUrl.IsAbsoluteUri)
				return BadRequest($"URL \"{pageUrl}\" не является абсолютным.");

			var result = await _indexer.IndexAsync(pageUrl);

			return Ok(result);
		}
	}
}