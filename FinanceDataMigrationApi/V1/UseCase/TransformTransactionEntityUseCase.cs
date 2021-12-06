using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    internal class TransformTransactionEntityUseCase : ITransformTransactionEntityUseCase
    {
        private IMigrationRunDynamoGateway _migrationRunGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IMapper _autoMapper;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformTransactionEntityUseCase(
            IMapper autoMapper,
            IMigrationRunDynamoGateway migrationRunGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway)
        {
            _autoMapper = autoMapper;
            _migrationRunGateway = migrationRunGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
        }

        // Read data from staging table and enrich with remaining subset data. i.e. Person
        // SuspensionInfo is null, this is only enriched by new system..
        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");

            try
            {
                // Get latest migrationrun item from DynamoDB Table MigrationRuns with status ExtractCompleted, where is_feature_enabled flag is TRUE.
                var migrationRun = await _migrationRunGateway.GetMigrationRunByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                // If there are rows to transform THEN
                if (migrationRun.ExpectedRowsToMigrate > 0)
                {
                    // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                    migrationRun.LastRunStatus = MigrationRunStatus.TransformInprogress;
                    await _migrationRunGateway.UpdateAsync(migrationRun).ConfigureAwait(false);

                    // Get all the Transaction entity extracted data from the SOW2b SQL Server database table DMEntityTransaction,
                    //      where isTransformed flag is FALSE and isLoaded flag is FALSE
                    var dMTransactions = await _dMTransactionEntityGateway.ListAsync().ConfigureAwait(false);


                    // We may want to get all the Persons (Id, FullName) and cache them.
                    var personsCache = GetPersonsCacheAsync();

                    //      Iterate through each row (or batched) and enrich with missing information for subsets
                    //          Get Person subset information (from above cached list)
                    //          Get SuspensionInfo subset information - dont know where this comes from?
                    //          Set the row isTransformed flag to TRUE and Update the row in the staging data table (or batch them)

                    //          Update batched rows to staging table.

                    // If all expected rows equal actual rows have been transformed THEN
                    //      Update migrationrun item with SET start_row_id & end_row_id here and set status to "TransformCompleted"

                }

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

        private static async Task<List<Person>> GetPersonsCacheAsync()
        {
            //TODO

            var personsCache = new List<Person>()
            {
                new Person { Id = Guid.NewGuid(), FullName = "TestFullName" }
            };

            return await Task.FromResult(personsCache).ConfigureAwait(false);

        }
    }
}
