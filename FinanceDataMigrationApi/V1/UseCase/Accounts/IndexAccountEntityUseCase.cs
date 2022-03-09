using FinanceDataMigrationApi.V1.Boundary.Response;
using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class IndexAccountEntityUseCase : IIndexAccountEntityUseCase
    {
        /*private readonly IEsGateway _esGateway;
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IAccountsGateway _accountsGateway;

        *//*private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "INDEXING";*//*

        public IndexAccountEntityUseCase(IEsGateway esGateway, IDMRunLogGateway dMRunLogGateway, IAccountsGateway accountsGateway)
        {
            _esGateway = esGateway;
            _dMRunLogGateway = dMRunLogGateway;
            _accountsGateway = accountsGateway;
        }*/

        public Task<StepResponse> ExecuteAsync()
        {

            throw new NotImplementedException();

            /*LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "IndexInprogress"
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                // Update migrationrun item with set status to "IndexInprogress". 
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexInprogress.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Get all the Account entities extracted data from the SOW2b SQL Server database table DMEntityAccounts,
                //      where isTransformed flag is TRUE and isLoaded flag is TRUE 
                var loadedAccounts = await _accountsGateway.GetLoadedListAsync().ConfigureAwait(false);

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} Entity");

                if (!loadedAccounts.Any())
                {
                    LoggingHandler.LogInfo($"No records to {DataMigrationTask} for {DMEntityNames.Transactions} Entity");
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.NothingToMigrate.ToString();
                }
                else
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var esRequests = EsFactory.
                        ToAccountRequestList(loadedAccounts);

                    stopwatch.Stop();
                    var time = stopwatch.Elapsed;
                    await _esGateway.BulkIndexAccounts(esRequests).ConfigureAwait(false);

                    // we need to update the corresponding rows isLoaded flag in the staging table.
                    loadedAccounts.ToList().ForEach(item => item.IsIndexed = true);

                    // Update batched rows to staging table DMTransactionEntity. 
                    await _accountsGateway.UpdateDMAccountEntityItems(loadedAccounts).ConfigureAwait(false);

                    // Update migrationrun item with SET start_row_id & end_row_id here.
                    //      and set status to "IndexCompleted" (Data Set Indexed successfully)
                    dmRunLogDomain.ActualRowsMigrated = loadedAccounts.Count;
                    dmRunLogDomain.StartRowId = loadedAccounts.First().Id;
                    dmRunLogDomain.EndRowId = loadedAccounts.Last().Id;
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.IndexCompleted.ToString();
                }

                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

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
            }*/
        }
    }
}
