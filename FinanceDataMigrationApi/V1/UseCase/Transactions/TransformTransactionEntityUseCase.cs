using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TransactionPerson = FinanceDataMigrationApi.V1.Domain.TransactionPerson;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class TransformTransactionEntityUseCase : ITransformTransactionEntityUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMTransactionEntityGateway _dMTransactionEntityGateway;
        private readonly ITenureGateway _tenureGateway;
        private readonly IPersonGateway _personGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";
        private Guid _targetId;

        public TransformTransactionEntityUseCase(
            IDMRunLogGateway dMRunLogGateway,
            IDMTransactionEntityGateway dMTransactionEntityGateway,
            ITenureGateway tenureGateway,
            IPersonGateway personGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMTransactionEntityGateway = dMTransactionEntityGateway;
            _tenureGateway = tenureGateway;
            _personGateway = personGateway;
        }

        // Read data from staging table and enrich with remaining subset data. i.e. Person
        // SuspensionInfo is null, this is only enriched by new system..
        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Transactions} entity");
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
                    transaction.TargetId = _targetId;
                    // Set the row isTransformed flag to TRUE and Update the row in the staging data table (or batch them)
                    transaction.IsTransformed = true;
                    transaction.IsIndexed = false;
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

        private static async Task<string> TransformTransactionType(string transactionType)
        {
            var enumValueString = EnumExtensions.GetValueFromDescription<TransactionType>(transactionType.Trim());
            return await Task.FromResult(enumValueString.ToString()).ConfigureAwait(false);
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


            var householdMembers = tenureList.SelectMany(x => x.HouseholdMembers).ToList();

            var householdMember = householdMembers.Where(x => x.IsResponsible).ToList();

            if (householdMember.Count == 1)
            {
                var transactionPerson = new TransactionPerson { Id = householdMember[0].Id, FullName = householdMember[0].FullName };
                return await Task.FromResult(JsonConvert.SerializeObject(transactionPerson)).ConfigureAwait(false);
            }

            return null;
        }
    }
}
