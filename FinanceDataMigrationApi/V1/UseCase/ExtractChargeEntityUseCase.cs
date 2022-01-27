using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
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
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");


        private const string DataMigrationTask = "EXTRACT";

        public ExtractChargeEntityUseCase(
            IDMRunLogGateway dmRunLogGateway,
            IChargeGateway dMChargeGateway)
        {
            _dMRunLogGateway = dmRunLogGateway;
            _dMChargeGateway = dMChargeGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Charges} entity");

            // Get latest successfull migrationrun item from Table MigrationRuns. where is_feature_enabled flag is TRUE.
            var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Charges).ConfigureAwait(false) ??
                                 new DMRunLogDomain() { DynamoDbTableName = DMEntityNames.Charges };

            // Get latest run timestamp from migrationrun item
            var lastRunTimestamp = dmRunLogDomain.LastRunDate ?? DateTimeOffset.UtcNow;

            // Update migrationrun item with latest run time to NOW and set status to "Extract Inprogress"
            dmRunLogDomain.LastRunDate = DateTimeOffset.UtcNow;
            dmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractInprogress.ToString();

            var newDMRunLogDomain = await _dMRunLogGateway.AddAsync(dmRunLogDomain).ConfigureAwait(false);

            // Call stored procedure usp_ExtractTransactionEntity in SOW2b database to kick off the extract of data to staging table using
            var numberOfRowsExtracted = await _dMChargeGateway.ExtractAsync(lastRunTimestamp).ConfigureAwait(false);

            // if return value from usp is >0 (success), then capture how many rows to migrate from return value.
            // if return value from usp is =0 (success), but no rows to migrate.
            // Update migrationrun item with latest run time to NOW and set status to "Extract Completed"
            if (numberOfRowsExtracted > 0)
            {
                // Update migrationrun item with latest run time to NOW and set status to "Extract Completed"
                newDMRunLogDomain.ExpectedRowsToMigrate = numberOfRowsExtracted;
                newDMRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();
                LoggingHandler.LogInfo($"Number of rows extracted for this migration run = [{numberOfRowsExtracted}]");
            }
            else
            {
                // if return value from usp is = -1 (usp returned failure).
                // error occurred in extract data stored procedure
                newDMRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();
                LoggingHandler.LogInfo($"Error occurred during {DataMigrationTask} task for {DMEntityNames.Charges} entity");
            }

            await _dMRunLogGateway.UpdateAsync(newDMRunLogDomain).ConfigureAwait(false);

            LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Charges} entity");

            return new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
            };

        }
    }
}
