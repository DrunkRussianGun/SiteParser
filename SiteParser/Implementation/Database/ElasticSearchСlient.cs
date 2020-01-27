﻿using System.Linq;
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

        public async Task<IndexedPage[]> FindAsync(string domainUrl)
        {
            var response = await _client.SearchAsync<IndexedPage>(search => search
                .Query(q => q
                    .Term(t => t
                        .Field(page => page.Url)
                        .Value(domainUrl)
                    )
                )
            );

            return response.Documents.ToArray();
        }
    }
}
