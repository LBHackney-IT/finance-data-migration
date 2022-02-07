using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class ExtractAccountEntityUseCase : IExtractAccountEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMAccountEntityGateway _dMAccountEntityGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "25";

        private const string DataMigrationTask = "EXTRACT";

        public ExtractAccountEntityUseCase(
            IDMRunLogGateway dmRunLogGateway,
            IDMAccountEntityGateway dMAccountEntityGateway)
        {
            _dMRunLogGateway = dmRunLogGateway;
            _dMAccountEntityGateway = dMAccountEntityGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                var lastRunTimestamp = dmRunLogDomain.LastRunDate;

                dmRunLogDomain.LastRunDate = DateTimeOffset.UtcNow;
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractInprogress.ToString();

                var newDmRunLogDomain = await _dMRunLogGateway.AddAsync(dmRunLogDomain).ConfigureAwait(false);

                var numberOfRowsExtracted = await _dMAccountEntityGateway.ExtractAsync(lastRunTimestamp).ConfigureAwait(false);

                if (numberOfRowsExtracted > 0)
                {
                    newDmRunLogDomain.ExpectedRowsToMigrate = numberOfRowsExtracted;
                    newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();
                    LoggingHandler.LogInfo($"Number of rows extracted for this migration run = [{numberOfRowsExtracted}]");
                }
                else
                {
                    newDmRunLogDomain.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();
                    LoggingHandler.LogInfo($"Error occurred during {DataMigrationTask} task for {DMEntityNames.Accounts} entity");
                }

                await _dMRunLogGateway.UpdateAsync(newDmRunLogDomain).ConfigureAwait(false);

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

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
