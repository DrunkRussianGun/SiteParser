using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteParser.Implementation.Scan
{
	public class UrlFilter
	{
		public HashSet<Uri> Filter(IEnumerable<Uri> urls, Uri domainUrl)
		{
			var host = domainUrl.Host;
			var hostWithDotBefore = '.' + host;
			var segments = GetSegments(domainUrl);
			return urls
				.Where(uri =>
					(uri.Host == domainUrl.Host || uri.Host.EndsWith(hostWithDotBefore)) &&
					GetSegments(uri).StartsWith(segments)
				)
				.ToHashSet();
		}

		private static string GetSegments(Uri url)
		{
			return string.Join("/", url.Segments);
		}
	}
}