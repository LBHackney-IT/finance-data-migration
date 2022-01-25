using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Microsoft.Extensions.Logging;
using Nest;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class EsGateway : IEsGateway
    {
        private readonly IElasticClient _esClient;
        private readonly ILogger<EsGateway> _logger;

        public EsGateway(IElasticClient esClient, ILogger<EsGateway> logger)
        {
            _esClient = esClient;
            _logger = logger;
        }
        public async Task<IndexResponse> IndexTransaction(QueryableTransaction transaction)
        {
            _logger.LogDebug($"Updating transaction index for transaction id {transaction.Id}");
            return await _esClient.IndexAsync(new IndexRequest<QueryableTransaction>(transaction, "transactions")).ConfigureAwait(false);
        }

        public Task BulkIndexTransaction(List<QueryableTransaction> transactions)
        {
            var response = _esClient.BulkAll(transactions, b => b
                    .Index("transactions")
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    _logger.LogDebug($"Indexing completed for Transactions");
                });
            return Task.CompletedTask;
        }
    }
}
