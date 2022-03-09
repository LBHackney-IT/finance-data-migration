using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionBulkIndexGateway : IBulkIndexGateway<QueryableTransaction>
    {
        private readonly IIndexFromIFStoFFSGateway<QueryableTransaction> _indexTransactionsFromIfStoFfsGateway;
        public TransactionBulkIndexGateway(IElasticClient elasticClient)
        {
            _indexTransactionsFromIfStoFfsGateway = new IndexTransactionsFromIfStoFfsGateway(elasticClient);
        }

        public async Task<Task> IndexAllAsync(List<QueryableTransaction> queryableList)
        {
            List<Task> tasks = new List<Task>();
            foreach (var queryableTransaction in queryableList)
            {
                tasks.Add(_indexTransactionsFromIfStoFfsGateway.IndexAsync(queryableTransaction));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.CompletedTask;
        }
    }
}
