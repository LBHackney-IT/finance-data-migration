using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    internal class TransformTransactionEntityUseCase : ITransformTransactionEntityUseCase
    {
        private IMigrationRunGateway _migrationRunGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly IMapper _autoMapper;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformTransactionEntityUseCase(
            IMapper autoMapper,
            IMigrationRunGateway migrationRunGateway,
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
                // Get latest migrationrun item from Table MigrationRuns with status ExtractCompleted, where is_feature_enabled flag is TRUE.
                var dmRunLogDomain = await _migrationRunGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                // If there are rows to transform THEN
                if (dmRunLogDomain.ExpectedRowsToMigrate > 0)
                {
                    // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformInprogress.ToString();
                    await _migrationRunGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                    // Get all the Transaction entity extracted data from the SOW2b SQL Server database table DMEntityTransaction,
                    //      where isTransformed flag is FALSE and isLoaded flag is FALSE
                    var dMTransactions = await _dMTransactionEntityGateway.ListAsync().ConfigureAwait(false);

                    // Iterate through each row (or batched) and enrich with missing information for subsets
                    foreach (var transaction in dMTransactions)
                    {
                        // Get Person subset information (from above cached list)
                        // We may want to get all the Persons (Id, FullName) and cache them.
                        transaction.Person = await GetPersonsCacheAsync(transaction.IdDynamodb, transaction.PaymentReference).ConfigureAwait(false);

                        // Set the row isTransformed flag to TRUE and Update the row in the staging data table (or batch them)
                        transaction.IsTransformed = true;
                    }

                    // If all expected rows equal actual rows have been transformed THEN
                    //      Update migrationrun item with SET start_row_id & end_row_id here and set status to "TransformCompleted"
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformCompleted.ToString();
                    await _migrationRunGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                    // Update batched rows to staging table DMTransactionEntity. 
                    await _dMTransactionEntityGateway.UpdateDMTransactionEntityItems(dMTransactions).ConfigureAwait(false);
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

        private static async Task<string> GetPersonsCacheAsync(Guid idDynamodb, string paymentReference)
        {
            //TODO temp method until decide how to get person information based on transaction entity
            return await Task.FromResult($"\"Id\" :\"{idDynamodb}\", \"FullName\" = \"{paymentReference}\"").ConfigureAwait(false);
        }
    }
}
