using AutoMapper;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi
{
    public class TransformTransactionEntityUseCase : ITransformTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformTransactionEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
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
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions).ConfigureAwait(false);

                // If there are rows to transform THEN
                if (dmRunLogDomain.ExpectedRowsToMigrate > 0)
                {
                    // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformInprogress.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                    // Get all the Transaction entity extracted data from the SOW2b SQL Server database table DMEntityTransaction,
                    //      where isTransformed flag is FALSE and isLoaded flag is FALSE
                    var dMTransactions = await _dMTransactionEntityGateway.ListAsync().ConfigureAwait(false);

                    // Iterate through each row (or batched) and enrich with missing information for subsets
                    foreach (var transaction in dMTransactions)
                    {
                        // Get Person subset information (from above cached list)
                        // We may want to get all the Persons (Id, FullName) and cache them.

                        // TODO FIX PERSON
                        transaction.Person = await GetPersonsCacheAsync(transaction.IdDynamodb, transaction.PaymentReference).ConfigureAwait(false);

                        transaction.TransactionType = await TransformTransactionType(transaction.TransactionType).ConfigureAwait(false);
                        transaction.TransactionSource = transaction.TransactionSource.Trim();
                        transaction.PaymentReference = transaction.PaymentReference.Trim();

                        // Set the row isTransformed flag to TRUE and Update the row in the staging data table (or batch them)
                        transaction.IsTransformed = true;
                    }

                    // Update batched rows to staging table DMTransactionEntity. 
                    await _dMTransactionEntityGateway.UpdateDMTransactionEntityItems(dMTransactions).ConfigureAwait(false);

                    // Update migrationrun item with set status to "TransformCompleted"
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformCompleted.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);
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

        private static async Task<string> TransformTransactionType(string transactionType)
        {
            //TODO Improve but this is the quick and dirty fix!
            var enumValueString = transactionType.Trim();
            return await Task.FromResult(Regex.Replace(enumValueString, "[^0-9A-Za-z]+", "")).ConfigureAwait(false);
        }

        private static async Task<string> GetPersonsCacheAsync(Guid idDynamodb, string paymentReference)
        {
            //TODO temp method until decide how to get person information based on transaction entity
            var tempPerson = new Person { Id = idDynamodb, FullName = paymentReference.Trim() };
            return await Task.FromResult(JsonConvert.SerializeObject(tempPerson)).ConfigureAwait(false);
        }
    }
}
