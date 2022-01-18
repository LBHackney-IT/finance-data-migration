using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Accounts;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public static class QueryResponseExtension
    {
        public static List<ConsolidatedCharge> ToConsolidatedChargeDomain(this QueryResponse response)
        {
            var consolidatedChargesList = new List<ConsolidatedCharge>();
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                var detailCharges = new List<DetailedCharges>();
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
