using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.Tenure.Domain;
using Hackney.Shared.Tenure.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using AccountTenureType = Hackney.Shared.HousingSearch.Domain.Accounts.TenureType;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class TransformAccountsUseCase : ITransformAccountsUseCase
    {
        private readonly IDMRunLogGateway _dMRunLogGateway;
        private readonly IDMAccountEntityGateway _dMAccountEntityGateway;
        private readonly IEsGateway _esGateway;
        private readonly ITenureDynamoDbGateway _tenureDynamoDbGateway;
        private readonly IConsolidatedChargesApiGateway _consolidatedChargesApiGateway;
        private readonly ITransactionsDynamoDbGateway _transactionsDynamoDbGateway;

        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");
        private const string DataMigrationTask = "TRANSFORM";

        public TransformAccountsUseCase(IDMRunLogGateway dMRunLogGateway,
            IDMAccountEntityGateway dMAccountEntityGateway,
            IEsGateway esGateway,
            ITenureDynamoDbGateway tenureDynamoDbGateway,
            IConsolidatedChargesApiGateway consolidatedChargesApiGateway,
            ITransactionsDynamoDbGateway transactionsDynamoDbGateway)
        {
            _dMRunLogGateway = dMRunLogGateway;
            _dMAccountEntityGateway = dMAccountEntityGateway;
            _esGateway = esGateway;
            _tenureDynamoDbGateway = tenureDynamoDbGateway;
            _consolidatedChargesApiGateway = consolidatedChargesApiGateway;
            _transactionsDynamoDbGateway = transactionsDynamoDbGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            LoggingHandler.LogInfo($"Starting {DataMigrationTask} task for {DMEntityNames.Accounts} entity");

            try
            {
                var dmRunLogDomain = await _dMRunLogGateway.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts).ConfigureAwait(false);

                if (dmRunLogDomain.ExpectedRowsToMigrate > 0)
                {
                    // Update migrationrun item with set status to "Transform Inprogress". SET start_row_id & end_row_id here or during LOAD?
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformInprogress.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);

                    var dMAccountEntities = await _dMAccountEntityGateway.ListAsync().ConfigureAwait(false);

                    // Iterate through each row (or batched) and enrich with missing information for subsets
                    foreach (var account in dMAccountEntities)
                    {
                        await PopulateAccount(account).ConfigureAwait(false);

                        account.IsTransformed = true;
                        account.IsIndexed = false;
                    }

                    // Update batched rows to staging table DMTransactionEntity.
                    await _dMAccountEntityGateway.UpdateDMAccountEntityItems(dMAccountEntities).ConfigureAwait(false);

                    // Update migrationrun item with set status to "TransformCompleted"
                    dmRunLogDomain.LastRunStatus = MigrationRunStatus.TransformCompleted.ToString();
                    await _dMRunLogGateway.UpdateAsync(dmRunLogDomain).ConfigureAwait(false);
                }

                LoggingHandler.LogInfo($"End of {DataMigrationTask} task for {DMEntityNames.Accounts} Entity");

                return new StepResponse()
                {
                    Continue = true,
                    NextStepTime = DateTime.Now.AddSeconds(int.Parse(_waitDuration))
                };
            }
            catch (Exception ex)
            {
                var namespaceLabel = $"{nameof(FinanceDataMigrationApi)}.{nameof(TransformAccountsUseCase)}.{nameof(ExecuteAsync)}";

                LoggingHandler.LogError($"{namespaceLabel} Application error");
                LoggingHandler.LogError(ex.ToString());

                throw;
            }
        }

        private async Task PopulateAccount(Infrastructure.Accounts.DMAccountEntity account)
        {
            var esTenure = await _esGateway.GetTenureByPrn(account.PaymentReference.Trim()).ConfigureAwait(false);

            account.TargetId = Guid.Parse(esTenure.Id);
            account.TargetType = Hackney.Shared.HousingSearch.Domain.Accounts.Enum.TargetType.Tenure.ToString();

            var tenureFromDynamoDb = await _tenureDynamoDbGateway.GetTenureById(account.TargetId.Value).ConfigureAwait(false);

            account.EndReasonCode = tenureFromDynamoDb.Terminated?.ReasonForTermination;
            account.AgreementType = "Master Account";
            account.ParentAccountId = null;
            account.AccountType = "Master";
            account.Tenure = JsonConvert.SerializeObject(ConstructTenure(tenureFromDynamoDb));

            if (tenureFromDynamoDb.TenuredAsset != null)
            {
                var colsolidateCharges = await _consolidatedChargesApiGateway
                    .GetConsolidatedtChargesByIdAsync(tenureFromDynamoDb.TenuredAsset.Id)
                    .ConfigureAwait(false);

                var accountCharges = colsolidateCharges.Select(charge => new QueryableConsolidatedCharge()
                {
                    Amount = charge.Amount,
                    Frequency = charge.Frequency,
                    Type = charge.Type
                }).ToList();

                account.ConsolidatedCharges = JsonConvert.SerializeObject(accountCharges);
            }
            account.AccountBalance = await CalculateBalanse(tenureFromDynamoDb.Id).ConfigureAwait(false);
            account.ConsolidatedBalance = account.AccountBalance;
        }

        private async Task<decimal> CalculateBalanse(Guid tenureId)
        {
            var transactions = await _transactionsDynamoDbGateway
                .GetTenureByTenureId(tenureId)
                .ConfigureAwait(false);

            return transactions.Sum(t => t.TransactionAmount);
        }

        private static QueryableTenure ConstructTenure(TenureInformationDb tenureEntity)
        {
            var accountTenure = new QueryableTenure()
            {
                FullAddress = tenureEntity.TenuredAsset?.FullAddress,
                TenureId = tenureEntity.Id.ToString(),
                TenureType = ConstructTenureType(tenureEntity.TenureType),
                PrimaryTenants = tenureEntity.HouseholdMembers.Select(hm => ConstructPrimaryTenant(hm)).ToList(),
            };

            return accountTenure;
        }

        private static QueryablePrimaryTenant ConstructPrimaryTenant(HouseholdMembers householdMember)
        {
            var primaryTenant = new QueryablePrimaryTenant()
            {
                Id = householdMember.Id,
                FullName = householdMember.FullName
            };

            return primaryTenant;
        }

        private static AccountTenureType ConstructTenureType(TenureType tenureType)
        {
            var accountTenureType = AccountTenureType.Create(tenureType.Code, tenureType.Description);

            return accountTenureType;
        }
    }
}
