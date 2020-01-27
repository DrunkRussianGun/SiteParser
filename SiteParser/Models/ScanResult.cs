using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteParser.Models
{
	public class ScanResult
	{
		public Uri[] PagesUrls { get; }

		public ScanResult(IEnumerable<Uri> urls)
		{
			PagesUrls = urls.ToArray();
		}
	}
}