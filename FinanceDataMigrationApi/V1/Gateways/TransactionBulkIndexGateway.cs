using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Hackney.Core.Sns;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Nest;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TransactionBulkIndexGateway : IBulkIndexGateway</*QueryableTransaction*/Transaction>
    {
        /*private readonly IIndexFromIFStoFFSGateway<QueryableTransaction> _indexTransactionsFromIfStoFfsGateway;*/
        private readonly ISnsGateway _snsGateway = new SnsGateway(new AmazonSimpleNotificationServiceClient());
        readonly TransactionSnsFactory _snsFactory;

        public TransactionBulkIndexGateway(IElasticClient elasticClient)
        {
            /*_indexTransactionsFromIfStoFfsGateway = new IndexTransactionsFromIfStoFfsGateway(elasticClient);*/
            _snsFactory = new TransactionSnsFactory();
        }

        public async Task<Task> IndexAllAsync(List</*QueryableTransaction*/Transaction> queryableList)
        {
            /*List<Task> tasks = new List<Task>();
            foreach (var queryableTransaction in queryableList)
            {
                tasks.Add(_indexTransactionsFromIfStoFfsGateway.IndexAsync(queryableTransaction));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.CompletedTask;*/
            foreach (var queryableTransaction in queryableList)
            {
                DatabaseContext context = DatabaseContext.Create();

                var transactionSnsMessage = _snsFactory.Create(queryableTransaction);
                var transactionTopicArn = Environment.GetEnvironmentVariable("TRANSACTION_SNS_ARN");
                await _snsGateway.Publish(transactionSnsMessage, transactionTopicArn, EventConstants.MESSAGEGROUPID).ConfigureAwait(false);
                var transaction = context.TransactionEntities
                    .FirstOrDefault(t => t.IdDynamodb == queryableTransaction.Id);

                if (transaction != null)
                {
                    transaction.MigrationStatus = EMigrationStatus.Indexed;
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            return Task.CompletedTask;
        }
    }
}
