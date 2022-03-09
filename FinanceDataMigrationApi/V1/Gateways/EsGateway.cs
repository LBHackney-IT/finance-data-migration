using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Nest;
using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Handlers;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class EsGateway<T> : IEsGateway<T>
        where T : class
    {
        private readonly IElasticClient _esClient;
        private readonly string _indexName;

        public EsGateway(IElasticClient esClient, string indexName)
        {
            _esClient = esClient;
            _indexName = indexName;
        }

        public async Task<IndexResponse> IndexAsync(T esObject)
        {
            if (esObject is null) throw new ArgumentNullException(nameof(esObject));
            var indexResponse = await _esClient.IndexAsync(new IndexRequest<T>(esObject, _indexName));
            LoggingHandler.LogInfo($"Indexing status:{indexResponse.Result}");
            return indexResponse;
        }
    }
}
