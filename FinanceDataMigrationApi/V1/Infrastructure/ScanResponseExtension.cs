using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public static class ScanResponseExtension
    {
        public static IEnumerable<TenureInformation> ToTenureInformation(this ScanResponse response)
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
    }
}
