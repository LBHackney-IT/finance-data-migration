using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Accounts;
using Hackney.Shared.Tenure.Domain;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public static class QueryResponseExtension
    {
        public static IEnumerable<TenureInformation> ToTenureInformation(this QueryResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                yield return new TenureInformation
                {
                    Id = Guid.Parse(item["id"].S),
                    TenuredAsset = null,
                    TenureType = null,
                    InformHousingBenefitsForChanges = false,
                    HouseholdMembers = null,
                    PaymentReference = item["paymentReference"].S
                };
            }
        }

        /*public static List<Transaction> ToTransactions(this QueryResponse response)
        {
            List<Transaction> transactions = new List<Transaction>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                SuspenseResolutionInfo suspenseResolutionInfo = null;
                Person person = null;

                if (item.Keys.Any(p => p == "suspense_resolution_info"))
                {
                    var innerItem = item["suspense_resolution_info"].M;
                    suspenseResolutionInfo = new SuspenseResolutionInfo
                    {
                        IsResolve = bool.Parse(innerItem["isResolve"].S),
                        Note = innerItem["note"].S,
                        ResolutionDate = DateTime.Parse(innerItem["resolutionDate"].S)
                    };  
                }

                if (item.Keys.Any(p => p == "person"))
                {
                    var innerItem = item["person"].M;
                    person = new Person
                    {
                        Id = Guid.Parse(innerItem["id"].S),
                        FullName = innerItem["fullName"].S
                    };
                }

                transactions.Add(new Transaction
                {

                    Id = Guid.Parse(item["id"].S),
                    Address = item["address"].S,
                    BalanceAmount = decimal.Parse(item["balance_amount"].N),
                    BankAccountNumber = item["bank_account_number"].S,
                    ChargedAmount = decimal.Parse(item["charged_amount"].N),
                    FinancialMonth = short.Parse(item["financial_month"].N),
                    FinancialYear = short.Parse(item["financial_year"].N),
                    Fund = item["fund"].S,
                    HousingBenefitAmount = decimal.Parse(item["housing_benefit_amount"].N),
                    IsSuspense = bool.Parse(item["is_suspense"].S),
                    PaidAmount = decimal.Parse(item["paid_amount"].N),
                    PaymentReference = item["payment_reference"].S,
                    PeriodNo = short.Parse(item["period_no"].N),
                    Person = person,
                    SuspenseResolutionInfo = suspenseResolutionInfo,
                    TargetId = Guid.Parse(item["target_id"].S),
                    TransactionAmount = decimal.Parse(item["transaction_amount"].N),
                    TransactionDate = DateTime.Parse(item["transaction_date"].S),
                    TransactionSource = item["transaction_source"].S,
                    TransactionType = Enum.Parse<TransactionType>(item["transaction_type"].S)
                });
            }

            return transactions;
        }*/

        public static List<ConsolidatedCharge> ToConsolidatedChargeDomain(this QueryResponse response)
        {
            var consolidatedChargesList = new List<ConsolidatedCharge>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                var detailCharges = new List<DmDetailedCharges>();
                var innerItem = item["detailed_charges"].L;
                foreach (var detail in innerItem)
                {
                    var type = detail.M["type"].S;
                    var frequency = detail.M["frequency"].S;
                    var amount = Convert.ToDecimal(detail.M["amount"].N);
                    var consolidatedCharge = ConsolidatedCharge.Create(type, frequency, amount);
                    consolidatedChargesList.Add(consolidatedCharge);
                }
            }

            return consolidatedChargesList;
        }
    }
}
