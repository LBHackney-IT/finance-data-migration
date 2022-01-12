using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class IndexAccountEntityUseCase : IIndexAccountEntityUseCase
    {
        private readonly IEsGateway _esGateway;
        private readonly IDMRunLogGateway _dMRunLogGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "INDEXING";

        public IndexAccountEntityUseCase(IEsGateway esGateway, IDMRunLogGateway dMRunLogGateway)
        {
            _esGateway = esGateway;
            _dMRunLogGateway = dMRunLogGateway;
        }


        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "IndexInprogress"
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                // Update migrationrun item with set status to "IndexInprogress". 
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Get all the Account entities extracted data from the SOW2b SQL Server database table DMEntityAccounts,
                //      where isTransformed flag is TRUE and isLoaded flag is FALSE
                //      populate the dynamodb Account table (using the Accounts API POST endpoint). Use a Batch mode. 
                var loadedList = await _dMTransactionEntityGateway.GetLoadedListAsync().ConfigureAwait(false);

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} Entity");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(ex.ToString());

                throw;
            }
        }
    }
}
