using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using Nest;
using SiteParser.Implementation;
using SiteParser.Implementation.Database;
using SiteParser.Models;

namespace SiteParser.Configuration
{
	 public static class ServiceCollectionExtensions
    {
        public static void AddElasticSearchDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var elasticSearchUrl = configuration.GetElasticSearchUrl();
            services.AddSingleton(serviceProvider =>
                new ElasticSearchClient(new ConnectionSettings(elasticSearchUrl)
                    .ThrowExceptions()
                    .DefaultMappingFor<ScannedPage>(mapping => mapping
                        .IndexName("pages")
                        .IdProperty(page => page.Url)
                    )
                )
            );
        }

        public static void AddImplementation(this IServiceCollection services)
        {
            services.AddSingleton<Searcher>();
            services.AddSingleton<Downloader>();
            services.AddSingleton<Scanner>();
        }

        public static void AddHttpClient(this IServiceCollection services, IConfiguration configuration, string clientName)
        {
            var headers = configuration.GetHttpClientHeaders();
            services.AddHttpClient(clientName, client =>
                headers.ForEach(header =>
                    client.DefaultRequestHeaders.Add(header.Key, header.Value)
                )
            );
        }
    }
}
