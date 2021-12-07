using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    internal class LoadTransactionEntityUseCase : ILoadTransactionEntityUseCase
    {
        private IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IMapper _autoMapper;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DataMigrationTask = "LOAD";

        public LoadTransactionEntityUseCase(

            IMapper autoMapper,
            IDMRunLogGateway dMRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway)
        {
            _autoMapper = autoMapper;
            _dMRunLogGateway = dMRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
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

                // *** START OF BLOCK ***
                // TODO *** Replace this call to Transaction API with batch of items. ***
                //      for each row from the Transformed List call Transaction API in batch mode,
                //      we need to update the corresponding rows isLoaded flag in the staging table. 
                foreach (var dmEntity in transformedList)
                {
                    // Call Transaction API for each item
                    var apiResult = await _dMTransactionEntityGateway.AddTransactionAsync(dmEntity).ConfigureAwait(false);
                    dmEntity.IsLoaded = true;
                }
                // *** END OF BLOCK

                // Update migrationrun item with SET start_row_id & end_row_id here.
                //      and set status to "LoadCompleted" (Data Set Migrated successfully)
                dmRunLogDomain.StartRowId = transformedList.First().Id;
                dmRunLogDomain.EndRowId = transformedList.Last().Id;
                dmRunLogDomain.LastRunStatus = MigrationRunStatus.LoadCompleted.ToString();
                await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                // Update batched rows to staging table DMTransactionEntity. 
                await _dMTransactionEntityGateway.UpdateDMTransactionEntityItems(transformedList).ConfigureAwait(false);

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
