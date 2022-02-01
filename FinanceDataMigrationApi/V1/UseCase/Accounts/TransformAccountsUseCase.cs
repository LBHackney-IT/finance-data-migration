using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class TransformAccountsUseCase : ITransformAccountsUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMAccountEntityGateway _dMAccountEntityGateway;
        private readonly IEsGateway _esGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformAccountsUseCase(IDMRunLogGateway dMRunLogGateway,
            IDMAccountEntityGateway dMAccountEntityGateway,
            IEsGateway esGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMAccountEntityGateway = dMAccountEntityGateway;
            _esGateway = esGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                if (dmRunLogDomain.ExpectedRowsToMigrate > 0)
                {
                    // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformInprogress.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                    var dMAccountEntities = await _dMAccountEntityGateway.ListAsync().ConfigureAwait(false);

                    // Iterate through each row (or batched) and enrich with missing information for subsets
                    foreach (var account in dMAccountEntities)
                    {
                        account.AgreementType = "Master Account";
                        account.ParentAccountId = null;
                        account.AccountType = "Master";
                        account.ConsolidatedBalance = account.AccountBalance;
                        account.IsTransformed = true;
                        account.IsIndexed = false;
                    }

                    // Update batched rows to staging table DMTransactionEntity.
                    await _dMAccountEntityGateway.UpdateDMAccountEntityItems(dMAccountEntities).ConfigureAwait(false);

                    // Update migrationrun item with set status to "TransformCompleted"
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformCompleted.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);
                }

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} Entity");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(TransformAccountsUseCase)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(ex.ToString());

                throw;
            }
        }
    }
}
