using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using SiteParser.Implementation;
using SiteParser.Implementation.Database;

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
                )
            );
        }

        public static void AddImplementation(this IServiceCollection services)
        {
            services.AddSingleton<Searcher>();
        }
    }
}
