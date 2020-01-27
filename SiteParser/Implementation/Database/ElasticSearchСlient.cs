using System.Linq;
using System.Threading.Tasks;
using Nest;
using SiteParser.Models;

namespace SiteParser.Implementation.Database
{
    public class ElasticSearchClient
    {
        private readonly ElasticClient _client;
        
        public ElasticSearchClient(IConnectionSettingsValues connectionSettings)
        {
            _client = new ElasticClient(connectionSettings);
        }

        public async Task<ScannedPage[]> FindAsync(string domainUrl)
        {
            var response = await _client.SearchAsync<ScannedPage>(search => search
                .Query(q => q
                    .Term(t => t
                        .Field(page => page.Url)
                        .Value(domainUrl)
                    )
                )
            );

            return response.Documents.ToArray();
        }

        public async Task<ScannedPage[]> GetAllAsync()
        {
            var response = await _client.SearchAsync<ScannedPage>();

            return response.Documents.ToArray();
        }

        public async Task InsertAsync(ScannedPage page)
        {
            var response = await _client.IndexAsync(page, index => index
                .Id(page.Url.ToString())
            );
        }
    }
}
