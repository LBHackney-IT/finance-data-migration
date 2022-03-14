using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class EsGateway : IEsGateway
    {
        private readonly IElasticClient _esClient;

        public EsGateway(IElasticClient esClient)
        {
            _esClient = esClient;
        }

        public async Task<IndexResponse> IndexTransaction(QueryableTransaction transaction)
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
                /*.Size(1000)*/)
                    .Wait(TimeSpan.FromMinutes(15), next =>
                    {
                        LoggingHandler.LogInfo($"Indexing completed for Transactions");
                    });
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                LoggingHandler.LogError($"{nameof(EsGateway)}.{nameof(BulkIndexTransaction)} Exception: {e.GetFullMessage()}");
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
        }
    }
}
