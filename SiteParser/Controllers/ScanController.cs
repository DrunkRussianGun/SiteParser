using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteParser.Implementation;

namespace SiteParser.Controllers
{
	public class ScanController : ControllerBase
	{
		private readonly Scanner _scanner;

		public ScanController(Scanner scanner)
		{
			_scanner = scanner;
		}
		
		[HttpPost]
		public async Task<IActionResult> Scan(
			[FromQuery] Uri pageUrl,
			[FromQuery] int maxDepth,
			[FromQuery] int maxLinksOnPageCount)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!pageUrl.IsAbsoluteUri)
				return BadRequest($"URL \"{pageUrl}\" не является абсолютным.");

			// ReSharper disable PossibleInvalidOperationException
			var result = await _scanner.ScanAsync(pageUrl, maxDepth, maxLinksOnPageCount);
			// ReSharper restore PossibleInvalidOperationException

			return Ok(result);
		}
	}
}