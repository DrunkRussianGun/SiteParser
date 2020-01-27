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
		public async Task<IActionResult> Scan([FromQuery] Uri pageUrl)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			if (!pageUrl.IsAbsoluteUri)
				return BadRequest($"URL \"{pageUrl}\" не является абсолютным.");

			var result = await _scanner.ScanAsync(pageUrl);

			return Ok(result);
		}
	}
}