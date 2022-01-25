using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Factories;

namespace FinanceDataMigrationApi
{
    public class LoadTransactionEntityUseCase : ILoadTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly ITransactionGateway _transactionGateway;
        private readonly IEsGateway _esGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DataMigrationTask = "LOAD";

        public LoadTransactionEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway,
            ITransactionGateway transactionGateway,
            IEsGateway esGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
            _transactionGateway = transactionGateway;
            _esGateway = esGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            try
            {
                // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "TransformCompleted"
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                //      Update migrationrun item with set status to "LoadInprogress". 
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Get all the Transaction entity extracted data from the SOW2b SQL Server database table DMEntityTransaction,
                //      where isTransformed flag is TRUE and isLoaded flag is FALSE
                //      populate the dynamodb Transaction table (using the Transaction API POST endpoint). Use a Batch mode. 
                var transformedList = await _dMTransactionEntityGateway.GetTransformedListAsync().ConfigureAwait(false);

                // for each row from the Transformed List call Transaction API in batch mode,
                var transactionRequestList = transformedList.ToTransactionRequestList();

                if (transactionRequestList.Any())
                {
                    var response = await _transactionGateway.UpdateTransactionItems(transactionRequestList).ConfigureAwait(false);

                    if (response > 0)
                    {
                        // we need to update the corresponding rows isLoaded flag in the staging table.
                        transformedList.ToList().ForEach(item => item.IsLoaded = true);
                      
                        // Update batched rows to staging table DMTransactionEntity. 
                        await _dMTransactionEntityGateway.UpdateDMTransactionEntityItems(transformedList).ConfigureAwait(false);

                        // Update migrationrun item with SET start_row_id & end_row_id here.
                        //      and set status to "LoadCompleted" (Data Set Migrated successfully)
                        dmRunLogDomain.ActualRowsMigrated = response;
                        dmRunLogDomain.StartRowId = transformedList.First().Id;
                        dmRunLogDomain.EndRowId = transformedList.Last().Id;
                        dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadCompleted.ToString();
                    }
                    else
                    {
                        dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadFailed.ToString();
                    }
                }
                else
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity");
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.NothingToMigrate.ToString();
                }

                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

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
