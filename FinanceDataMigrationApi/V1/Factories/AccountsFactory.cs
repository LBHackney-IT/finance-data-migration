using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class AccountsFactory
    {
        public static Dictionary<string, AttributeValue> ToQueryRequest(this DMAccountEntity account)
        {
            var accountModel = new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = account.Id.ToString()}},
                {"account_balance", new AttributeValue {N = account.AccountBalance.HasValue ? account.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = account.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = account.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = account.AccountType.ToString()}},
                {"rent_group_type", new AttributeValue {S = account.RentGroupType.ToString()}},
                {"agreement_type", new AttributeValue {S = account.AgreementType.ToString()}},
                //{"created_by", new AttributeValue {S = account.CreatedBy}},
                //{"last_updated_by", new AttributeValue {S = account.LastUpdatedBy}},
                //{"created_at", new AttributeValue {S = account.CreatedAt.ToString()}},
                //{"last_updated_at", new AttributeValue {S = account.LastUpdatedAt.ToString()}},
                {"start_date", new AttributeValue {S = account.StartDate.ToString()}},
                {"end_date", new AttributeValue {S = account.EndDate.ToString()}},
                {"account_status", new AttributeValue {S = account.AccountStatus.ToString()}},
                {"payment_reference", new AttributeValue {S = account.PaymentReference}},
                {"parent_account_id", new AttributeValue {S = account.ParentAccountId.ToString()}}
            };

            if (!string.IsNullOrWhiteSpace(account.Tenure))
            {
                TenureDbEntity tenure = JsonConvert.DeserializeObject<TenureDbEntity>(account.Tenure);

                var tenureAttributes = new Dictionary<string, AttributeValue>
                {
                    { "tenureId", new AttributeValue { S = tenure.TenureId }},
                    { "fullAddress", new AttributeValue { S = tenure.FullAddress }}
                };

                if (tenure.TenureType != null)
                {
                    tenureAttributes.Add("tenureType", new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            {"code", new AttributeValue {S = tenure.TenureType.Code}},
                            {"description", new AttributeValue {S = tenure.TenureType.Description}},
                        }
                    });
                }

                if (tenure.PrimaryTenants != null)
                {
                    var primaryTenantsAttributes = new List<AttributeValue>(tenure.PrimaryTenants.Count);

                    foreach (var tenant in tenure.PrimaryTenants)
                    {
                        primaryTenantsAttributes.Add(new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                {"fullName", new AttributeValue {S = tenant.FullName }},
                                {"id", new AttributeValue {S = tenant.Id.ToString() }},
                            }
                        });
                    }

                    tenureAttributes.Add("primaryTenants", new AttributeValue { L = primaryTenantsAttributes });
                }

                accountModel.Add("tenure", new AttributeValue { M = tenureAttributes });
            }

            if(!string.IsNullOrWhiteSpace(account.ConsolidatedCharges))
            {
                var consolidatedCharges = JsonConvert.DeserializeObject<List<QueryableConsolidatedCharge>>(account.ConsolidatedCharges);

                var chargesAttributes = new List<AttributeValue>(consolidatedCharges.Count);
                foreach (var charge in consolidatedCharges)
                {
                    chargesAttributes.Add(new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                            {
                                {"amount", new AttributeValue {N = charge.Amount.ToString("F") }},
                                {"frequency", new AttributeValue {S = charge.Frequency }},
                                {"type", new AttributeValue {S = charge.Type }},
                            }
                    });
                }
            }

            return accountModel;
        }
    }
}
