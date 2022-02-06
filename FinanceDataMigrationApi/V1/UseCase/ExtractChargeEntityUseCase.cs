using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class ExtractChargeEntityUseCase : IExtractChargeEntityUseCase
    {

        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IChargeGateway _dMChargeGateway;


        private const string DataMigrationTask = "EXTRACT";

        public ExtractChargeEntityUseCase(
            IDMRunLogGateway dmRunLogGateway,
            IChargeGateway dMChargeGateway)
        {
            _dMRunLogGateway = dmRunLogGateway;
            _dMChargeGateway = dMChargeGateway;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Charges} entity");

                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Charges).ConfigureAwait(false) ??
                                     new DMRunLogDomain() { DynamoDbTableName = DMEntityNames.Charges };
                var lastRunTimestamp = dmRunLogDomain.LastRunDate ?? DateTimeOffset.UtcNow;
                dmRunLogDomain.LastRunDate = DateTimeOffset.UtcNow;
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractInprogress.ToString();

                var newDmRunLogDomain = await _dMRunLogGateway.AddAsync(dmRunLogDomain).ConfigureAwait(false);

                LoggingHandler.LogInfo($"Calling {DataMigrationTask} SQL Stored Procedure for {DMEntityNames.Charges} entity");
                var numberOfRowsExtracted = await _dMChargeGateway.ExtractAsync(lastRunTimestamp).ConfigureAwait(false);

                LoggingHandler.LogInfo($"{DataMigrationTask} SQL Stored Procedure executed successfully for {DMEntityNames.Charges} entity");
                if (numberOfRowsExtracted >= 0)
                {
                    newDmRunLogDomain.ExpectedRowsToMigrate = numberOfRowsExtracted;
                    newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();
                    LoggingHandler.LogInfo($"Number of rows extracted for this migration run = [{numberOfRowsExtracted}]");
                }
                else
                {
                    newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();
                    LoggingHandler.LogInfo($"Error occurred during {DataMigrationTask} task for {DMEntityNames.Charges} entity");
                }

                await _dMRunLogGateway.UpdateAsync(newDmRunLogDomain).ConfigureAwait(false);
                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Charges} entity");

            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(DataMigrationTask)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error: {exc.Message}");
                LoggingHandler.LogError(exc.ToString());
                throw;
            }
        }
    }
}
