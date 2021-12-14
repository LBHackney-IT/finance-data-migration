using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TransactionPerson = FinanceDataMigrationApi.V1.Domain.TransactionPerson;

namespace FinanceDataMigrationApi
{
    public class TransformTransactionEntityUseCase : ITransformTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly ITenureGateway _tenureGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";
        private Guid _targetId;

        public TransformTransactionEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway,
            ITenureGateway tenureGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
            _tenureGateway = tenureGateway;
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
                        transaction.Person = await GetTransactionPersonAsync(transaction.PaymentReference.Trim()).ConfigureAwait(false);
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
            try
            {
                var enumValueString = EnumExtensions.GetValueFromDescription<TransactionType>(transactionType.Trim());
                return await Task.FromResult(enumValueString.ToString()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// This gets the first responsible person
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <returns></returns>
        public async Task<string> GetTransactionPersonAsync(string paymentReference)
        {
            var tenureList = await _tenureGateway.GetByPrnAsync(paymentReference).ConfigureAwait(false);

            if (tenureList is null) return null;

            var tenure = tenureList.FirstOrDefault();
            _targetId = tenure.Id;
            var householdMembers = tenureList.Select(x => x.HouseholdMembers).FirstOrDefault();
            var householdMember = householdMembers!.FirstOrDefault(x => x.IsResponsible);

            if (householdMember != null)
            {
                var transactionPerson = new TransactionPerson { Id = householdMember.Id, FullName = householdMember.FullName };
                return await Task.FromResult(JsonConvert.SerializeObject(transactionPerson)).ConfigureAwait(false);
            }

            return null;
        }
    }
}
