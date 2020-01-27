using System;
using Microsoft.Extensions.Configuration;

namespace SiteParser.Configuration
{
	public static class ConfigurationExtensions
	{
		public const string ElasticSearchUrlKey = "ELASTICSEARCH_URL";
		
		public static Uri GetElasticSearchUrl(this IConfiguration configuration)
		{
			var url = Environment.GetEnvironmentVariable(ElasticSearchUrlKey);
			if (!string.IsNullOrEmpty(url))
				return new Uri(url);

			url = configuration.GetValue<string>(ElasticSearchUrlKey);
			return new Uri(url);
		}
	}
}