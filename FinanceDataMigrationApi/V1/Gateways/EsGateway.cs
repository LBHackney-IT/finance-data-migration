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

            LoggingHandler.LogInfo($"Indexing started");
            return await _esClient.IndexAsync(new IndexRequest<T>(esObject, _indexName));
        }

        /*public async Task<IndexResponse> IndexTransaction(QueryableTransaction transaction)
        {
            LoggingHandler.LogInfo($"Updating transaction index for transaction id {transaction.Id}");
            return await _esClient.IndexAsync(new IndexRequest<QueryableTransaction>(transaction, "transactions")).ConfigureAwait(false);
        }

        public Task BulkIndexTransaction(List<QueryableTransaction> transactions)
        {
            try
            {
                var response = _esClient.BulkAll(transactions, b => b
                .Index("transactions")
                .BackOffTime("30s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000))
                    .Wait(TimeSpan.FromMinutes(15), next =>
                    {
                        LoggingHandler.LogInfo($"Indexing completed for Transactions");
                    });
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{nameof(EsGateway<T>)}.{nameof(BulkIndexTransaction)} Exception: {e.GetFullMessage()}");
                throw;
            }
        }

        public Task BulkIndexAccounts(List<QueryableAccount> accounts)
        {
            var response = _esClient.BulkAll(accounts, b => b
                    .Index("accounts")
                    .BackOffTime("30s")
                    .BackOffRetries(2)
                    .RefreshOnCompleted()
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    LoggingHandler.LogInfo($"Indexing completed for Accounts");
                });

            return Task.CompletedTask;
        }*/
    }
}
