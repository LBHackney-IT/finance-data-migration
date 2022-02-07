using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class ExtractTransactionEntityUseCase : IExtractTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly ITransactionGateway _transactionGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DataMigrationTask = "EXTRACT";

        public ExtractTransactionEntityUseCase(
            IDMRunLogGateway dmRunLogGateway,
            ITransactionGateway transactionGateway)
        {
            _dMRunLogGateway = dmRunLogGateway;
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false) ??
                                 new DMRunLogDomain()
                                 {
                                     DynamoDbTableName = DMEntityNames.Transactions
                                 };

            var lastRunTimestamp = dmRunLogDomain?.LastRunDate;

            dmRunLogDomain.LastRunDate = DateTimeOffset.UtcNow;
            dmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractInprogress.ToString();

            var newDmRunLogDomain = await _dMRunLogGateway.AddAsync(dmRunLogDomain).ConfigureAwait(false);

            var numberOfRowsExtracted = await _transactionGateway.ExtractAsync(lastRunTimestamp).ConfigureAwait(false);

            if (numberOfRowsExtracted > 0)
            {
                newDmRunLogDomain.ExpectedRowsToMigrate = numberOfRowsExtracted;
                newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();
                LoggingHandler.LogInfo($"Number of rows extracted for this migration run = [{numberOfRowsExtracted}]");
            }
            else
            {
                newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();
                LoggingHandler.LogInfo($"Error occurred during {DataMigrationTask} task for {DMEntityNames.Transactions} entity");
            }

            await _dMRunLogGateway.UpdateAsync(newDmRunLogDomain).ConfigureAwait(false);

            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };
        }
    }
}
