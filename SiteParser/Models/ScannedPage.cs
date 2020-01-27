using System;

namespace SiteParser.Models
{
	public class ScannedPage
	{
		public Uri Url { get; }
		
		public string Text { get; }

		public ScannedPage(Uri url, string text)
		{
			Url = url;
			Text = text;
		}
	}
}