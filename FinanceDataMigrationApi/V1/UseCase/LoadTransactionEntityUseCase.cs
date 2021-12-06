using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    internal class LoadTransactionEntityUseCase : ILoadTransactionEntityUseCase
    {
        private IMigrationRunDynamoGateway _migrationRunGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IMapper _autoMapper;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private const string DataMigrationTask = "LOAD";

        public LoadTransactionEntityUseCase(

            IMapper autoMapper,
            IMigrationRunDynamoGateway migrationRunGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway)
        {
            _autoMapper = autoMapper;
            _migrationRunGateway = migrationRunGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            await Task.Delay(0).ConfigureAwait(false);

            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            try
            {
                // Get latest successfull migrationrun item from DynamoDB Table MigrationRuns. where is_feature_enabled flag is TRUE and set status is "TransformCompleted"
                //      Update migrationrun item with set status to "LoadInprogress". 

                // Get data from staging table and populate the dynamodb Transaction table (using the Transaction API POST endpoint).
                //      Use a Batch mode. 

                //      for each row loaded into dynamodb Transaction table using the Transaction API in batch mode,
                //      we need to update the corresponding rows isLoaded flag. 

                // If all expected rows equal actual rows have been loaded THEN
                //      Update migrationrun item with SET start_row_id & end_row_id here?
                //      and set status to "LoadCompleted" (Data Set Migrated successfully)


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
