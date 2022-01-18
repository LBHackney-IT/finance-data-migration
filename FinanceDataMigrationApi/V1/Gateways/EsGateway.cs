using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Core.ElasticSearch.Interfaces;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QueryableTenure = Hackney.Shared.HousingSearch.Gateways.Models.Tenures.QueryableTenure;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class EsGateway : IEsGateway
    {
        private readonly IElasticClient _esClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EsGateway> _logger;

        public EsGateway(IElasticClient esClient,
            ILogger<EsGateway> logger,
            IServiceProvider serviceProvider)
        {
            _esClient = esClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                    _logger.LogDebug($"Indexing completed for Accounts");
                });

            return Task.CompletedTask;
        }

        public async Task<QueryableTenure> GetTenureByPrn(string prn)
        {
            var querybuilder = _serviceProvider.GetService<IQueryBuilder<QueryableTenure>>();

            var tenures = await _esClient.SearchAsync<QueryableTenure>(x =>
                x.Index(Indices.Index(new List<IndexName> { "tenures" }))
                .Query(q => querybuilder.WithWildstarQuery(prn, new List<string>
                {
                    "paymentReference"
                }).Build(q))).ConfigureAwait(false);

            if (tenures == null || tenures.Documents == null || !tenures.Documents.Any())
            {
                throw new ArgumentException("Cannot find any tenures by PRN " + prn);
            }

            var tenure = tenures.Documents.FirstOrDefault(t => string.Equals(t.PaymentReference, prn));

            if (tenure == null)
            {
                throw new ArgumentException($"Tenure with provided PRN {prn} was not found");
            }

            return tenure;
        }
    }
}
