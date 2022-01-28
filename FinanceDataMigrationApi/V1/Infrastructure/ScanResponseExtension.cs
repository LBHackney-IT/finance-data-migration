using System;
using System.Collections.Generic;
using System.Linq;
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
                    TenuredAsset = item.ContainsKey("tenuredAsset") ? new TenuredAsset()
                    {
                        FullAddress = item["tenuredAsset"].M["fullAddress"].S
                    } : null,
                    TenureType = item.ContainsKey("tenureType") ? new TenureType()
                    {
                        Code = item["tenureType"].M["code"].S,
                        Description = item["tenureType"].M["description"].S
                    } : null,
                    PaymentReference = item.ContainsKey("paymentReference") ? item["paymentReference"].S : null,
                    HouseholdMembers = item.ContainsKey("householdMembers") ? item["householdMembers"].L.ToArray().Select(m =>
                           new HouseholdMembers
                           {
                               Id = Guid.Parse(m.M["id"].S),
                               FullName = m.M["fullName"].S,
                               IsResponsible = m.M["isResponsible"].BOOL
                           }) : null
                };
            }
        }
    }
}
