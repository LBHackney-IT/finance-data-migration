using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class ExtractAccountEntityUseCase : IExtractAccountEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IAccountsGateway _accountsGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION") ?? "25";

        private const string DataMigrationTask = "EXTRACT";

        public ExtractAccountEntityUseCase(
            IDMRunLogGateway dmRunLogGateway,
            IAccountsGateway accountsGateway)
        {
            _dMRunLogGateway = dmRunLogGateway;
            _accountsGateway = accountsGateway;
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

                var numberOfRowsExtracted = await _accountsGateway.ExtractAsync().ConfigureAwait(false);

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
