using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteParser.Models
{
	public class IndexingResult
	{
		public Uri[] PagesUrls { get; }

		public IndexingResult(IEnumerable<Uri> urls)
		{
			PagesUrls = urls.ToArray();
		}
	}
}