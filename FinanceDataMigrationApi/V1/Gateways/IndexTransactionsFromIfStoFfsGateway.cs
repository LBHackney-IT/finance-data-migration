using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class IndexTransactionsFromIfStoFfsGateway : IIndexFromIFStoFFSGateway<QueryableTransaction>
    {
        private readonly IEsGateway<QueryableTransaction> _esGateway;

        public IndexTransactionsFromIfStoFfsGateway(IElasticClient elasticClient)
        {
            _esGateway = new EsGateway<QueryableTransaction>(elasticClient, "transactions");

        }

        public async Task IndexAsync(QueryableTransaction queryableObject)
        {
            DatabaseContext context = DatabaseContext.Create();
            var transaction = context.TransactionEntities
                .FirstOrDefault(t => t.IdDynamodb == queryableObject.Id);

            if (transaction != null)
            {
                await _esGateway.IndexAsync(queryableObject).ConfigureAwait(false);
                transaction.MigrationStatus = EMigrationStatus.Indexed;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                LoggingHandler.LogError($"The queryable transaction not exists!");
            }
        }
    }
}
