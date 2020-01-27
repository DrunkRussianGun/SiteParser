using System;

namespace SiteParser.Models
{
	public class IndexedPage
	{
		public Uri Url { get; }
		
		public string Text { get; }

		public IndexedPage(Uri url, string text)
		{
			Url = url;
			Text = text;
		}
	}
}