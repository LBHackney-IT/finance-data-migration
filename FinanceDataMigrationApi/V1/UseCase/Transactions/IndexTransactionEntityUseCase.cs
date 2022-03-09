using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class IndexTransactionEntityUseCase : IIndexTransactionEntityUseCase
    {
        private readonly ITransactionGateway _transactionGateway;
        private readonly IBulkIndexGateway</*QueryableTransaction*/Transaction> _esBulkTransactionGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "15";
        private const string DataMigrationTask = "INDEXING";

        public IndexTransactionEntityUseCase(ITransactionGateway dMTransactionEntityGateway, IElasticClient elasticClient)
        {
            _transactionGateway = dMTransactionEntityGateway;
            _esBulkTransactionGateway = new TransactionBulkIndexGateway(elasticClient);
        }
        public async Task<StepResponse> ExecuteAsync(int count)
        {
            DatabaseContext context = DatabaseContext.Create();
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            var loadedList = await _transactionGateway.GetLoadedListAsync(count).ConfigureAwait(false);

            var transactionRequestList = loadedList.ToTransactionRequestList();

            if (transactionRequestList.Any())
            {
                var esRequests = EsFactory.ToTransactionRequestList(transactionRequestList);
                await _esBulkTransactionGateway.IndexAllAsync(/*esRequests*/transactionRequestList).ConfigureAwait(false);
            }
            else
            {
                LoggingHandler.LogInfo($"There are no records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity");
                return new StepResponse()
                {
                    Continue = false
                };
            }

            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }
    }
}
