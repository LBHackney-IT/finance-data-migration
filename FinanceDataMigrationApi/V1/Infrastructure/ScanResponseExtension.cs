using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Handlers;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public static class ScanResponseExtension
    {
        public static IEnumerable<TenureInformation> ToTenureInformation(this ScanResponse response)
        {
            LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ToTenureInformation)}, ScanResponse: {response}");
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ToTenureInformation)}, response.Items.Count: {response.Items}");
                if (!item.ContainsKey("id"))
                    throw new Exception(response.ToString());

                yield return new TenureInformation
                {
                    Id = Guid.Parse(item["id"].S),
                    TenuredAsset = item.ContainsKey("tenuredAsset") ? new TenuredAsset()
                    {
                        FullAddress = item["tenuredAsset"].M.ContainsKey("fullAddress") ?
                            item["tenuredAsset"].M["fullAddress"].S : null
                    } : null,
                    TenureType = item.ContainsKey("tenureType") ? new TenureType()
                    {
                        Code = item["tenureType"].M.ContainsKey("code") ?
                            item["tenureType"].M["code"].S : null,
                        Description = item["tenureType"].M.ContainsKey("description") ?
                            item["tenureType"].M["description"].S : null
                    } : null,
                    Terminated = item.ContainsKey("terminated") ? new Terminated()
                    {
                        IsTerminated = item["terminated"].M.ContainsKey("isTerminated") && item["terminated"].M["isTerminated"].BOOL,
                        ReasonForTermination = item["terminated"].M.ContainsKey("reasonForTermination") ?
                            item["terminated"].M["reasonForTermination"].S.Trim() : null
                    } : null,
                    PaymentReference = item.ContainsKey("paymentReference") ?
                        item["paymentReference"].S : null,
                    HouseholdMembers = item.ContainsKey("householdMembers") ?
                        item["householdMembers"].L.Select(m =>
                           new HouseholdMembers
                           {
                               Id = Guid.Parse(m.M["id"].S),
                               FullName = m.M.ContainsKey("fullName") ?
                                   m.M["fullName"].S : null,
                               IsResponsible = m.M.ContainsKey("isResponsible") && m.M["isResponsible"].BOOL
                           }) : null
                };
            }
        }
    }
}
