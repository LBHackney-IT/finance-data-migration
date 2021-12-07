using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public class ExtractTransactionEntityUseCase : IExtractTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IMapper _autoMapper;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DataMigrationTask = "EXTRACT";

        public ExtractTransactionEntityUseCase(
            IMapper autoMapper,
            IDMRunLogGateway dmRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway)
        {
            _autoMapper = autoMapper;
            _dMRunLogGateway = dmRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            try
            {
                // Get latest successfull migrationrun item from Table MigrationRuns. where is_feature_enabled flag is TRUE.
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                // Get latest run timestamp from migrationrun item
                var lastRunTimestamp = dmRunLogDomain.LastRunDate;

                // Update migrationrun item with latest run time to NOW and set status to "Extract Inprogress"
                dmRunLogDomain.LastRunDate = DateTimeOffset.UtcNow; 
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractInprogress.ToString();

                var newDMRunLogDomain = await _dMRunLogGateway.AddAsync(dmRunLogDomain).ConfigureAwait(false);

                // Call stored procedure usp_ExtractTransactionEntity in SOW2b database to kick off the extract of data to staging table.
                int numberOfRowsExtracted = await _dMTransactionEntityGateway.ExtractAsync(lastRunTimestamp).ConfigureAwait(false);

                // if return value from usp is success (>0), then capture how many rows to migrate from return value.
                // if return value from usp is fail(=0), then no rows to migrate.
                // Update migrationrun item with latest run time to NOW and set status to "Extract Completed"
                if (numberOfRowsExtracted < 0) // -1 usp returned failure
                {
                    // error occurred in extract data stored procedure 
                    newDMRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();
                    LoggingHandler.LogInfo($"Error occurred during {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");
                }
                else
                {
                    // Update migrationrun item with latest run time to NOW and set status to "Extract Completed"
                    newDMRunLogDomain.ExpectedRowsToMigrate = numberOfRowsExtracted;
                    newDMRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();
                }

                await _dMRunLogGateway.UpdateAsync(newDMRunLogDomain).ConfigureAwait(false);

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Transactions} Entity");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };

            }
            catch (Exception exc)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(exc.ToString());

                throw;
            }
        }
    }
}
