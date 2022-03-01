using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class IndexTransactionEntityUseCase : IIndexTransactionEntityUseCase
    {
        /*private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly ITransactionGateway _transactionGateway;
        private readonly IEsGateway _esGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "INDEXING";*/

        /*public IndexTransactionEntityUseCase(IDMRunLogGateway dMRunLogGateway, ITransactionGateway dMTransactionEntityGateway, IEsGateway esGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _transactionGateway = dMTransactionEntityGateway;
            _esGateway = esGateway;
        }*/
        public Task<StepResponse> ExecuteAsync()
        {
            throw new NotImplementedException("Temporary disabled!");
            /*LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");
            
            var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

            dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexInprogress.ToString();
            await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

            var loadedList = await _transactionGateway.GetLoadedListAsync().ConfigureAwait(false);

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
                    await _transactionGateway.UpdateDMTransactionEntityItems(loadedList).ConfigureAwait(false);

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
            };*/
        }
    }
}
