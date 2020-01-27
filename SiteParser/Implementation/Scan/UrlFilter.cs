using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteParser.Implementation.Scan
{
	public class UrlFilter
	{
		public HashSet<Uri> Filter(IEnumerable<Uri> urls, Uri domainUrl, bool segmentsMustMatch = false)
		{
			var host = domainUrl.Host;
			var hostWithDotBefore = '.' + host;
			var segments = GetSegments(domainUrl);

			var result = urls
				.Where(uri => uri.Host == domainUrl.Host || uri.Host.EndsWith(hostWithDotBefore));
			if (segmentsMustMatch)
				result = result
					.Where(uri => GetSegments(uri).StartsWith(segments));
			return result.ToHashSet();
		}

		private static string GetSegments(Uri url)
		{
			return string.Join("/", url.Segments);
		}
	}
}