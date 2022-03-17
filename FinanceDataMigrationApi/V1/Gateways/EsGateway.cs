using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Handlers;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class EsGateway<T> : IEsGateway<T> where T : class
    {
        private readonly IElasticClient _esClient;
        private readonly string _indexName;

        public EsGateway(IElasticClient esClient, string indexName)
        {
            _esClient = esClient;
            _indexName = indexName;
        }

        public Task BulkIndex(IEnumerable<T> queryableEntity)
        {
            var response = _esClient.BulkAll(queryableEntity, b => b
                    .Index(_indexName)
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                )
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    LoggingHandler.LogInfo($"Indexing completed for {nameof(queryableEntity)}");
                });

            return Task.CompletedTask;
        }
    }
}
