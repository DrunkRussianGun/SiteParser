using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SiteParser.Configuration
{
	public static class ConfigurationExtensions
	{
		public const string ElasticSearchUrlKey = "ELASTICSEARCH_URL";
		public const string HttpClientHeadersKey = "HttpClientHeaders";
		
		public static Uri GetElasticSearchUrl(this IConfiguration configuration)
		{
			var url = Environment.GetEnvironmentVariable(ElasticSearchUrlKey);
			if (!string.IsNullOrEmpty(url))
				return new Uri(url);

			url = configuration.GetValue<string>(ElasticSearchUrlKey);
			return new Uri(url);
		}

		public static Dictionary<string, string> GetHttpClientHeaders(this IConfiguration configuration)
		{
			var headers = new Dictionary<string, string>();
			configuration
				.GetSection(HttpClientHeadersKey)
				.Bind("Headers", headers);
			return headers;
		}
	}
}