using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class IndexTransactionEntityUseCase : IIndexTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IEsGateway _esGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "INDEXING";

        public IndexTransactionEntityUseCase(IDMRunLogGateway dMRunLogGateway, IDMTransactionEntityGateway dMTransactionEntityGateway, IEsGateway esGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
            _esGateway = esGateway;
        }
        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");
            try
            {
                // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "IndexInprogress"
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                //      Update migrationrun item with set status to "IndexInprogress". 
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Get all the Transaction entity extracted data from the SOW2b SQL Server database table DMEntityTransaction,
                //      where isTransformed flag is TRUE and isLoaded flag is FALSE
                //      populate the dynamodb Transaction table (using the Transaction API POST endpoint). Use a Batch mode. 
                var loadedList = await _dMTransactionEntityGateway.GetLoadedListAsync().ConfigureAwait(false);

                // for each row from the Transformed List call Transaction API in batch mode,
                var transactionRequestList = loadedList.ToTransactionRequestList();

                if (transactionRequestList.Any())
                {

                    if (transactionRequestList.Count > 0)
                    {

                        // ES INDEXING

                        var esRequests = EsFactory.ToTransactionRequestList(transactionRequestList);
                        await _esGateway.BulkIndexTransaction(esRequests).ConfigureAwait(false);

                        // we need to update the corresponding rows isLoaded flag in the staging table.
                        loadedList.ToList().ForEach(item => item.IsIndexed = true);
                       
                        // Update batched rows to staging table DMTransactionEntity. 
                        await _dMTransactionEntityGateway.UpdateDMTransactionEntityItems(loadedList).ConfigureAwait(false);

                        // Update migrationrun item with SET start_row_id & end_row_id here.
                        //      and set status to "LoadCompleted" (Data Set Migrated successfully)
                        dmRunLogDomain.ActualRowsMigrated = transactionRequestList.Count;
                        dmRunLogDomain.StartRowId = loadedList.First().Id;
                        dmRunLogDomain.EndRowId = loadedList.Last().Id;
                        dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexCompleted.ToString();
                    }
                    else
                    {
                        dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexFailed.ToString();
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
