using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Infrastructure;
using Hackney.Shared.HousingSearch.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class ChargesDynamoDbGateway : IConsolidatedChargesApiGateway
    {
        private readonly IAmazonDynamoDB _amazonDynamoDb;

        public ChargesDynamoDbGateway(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
        }

        public async Task<List<ConsolidatedCharge>> GetConsolidatedtChargesByIdAsync(Guid targetId)
        {
            var request = new QueryRequest
            {
                TableName = "Charges",
                KeyConditionExpression = "target_id = :V_target_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":V_target_id",new AttributeValue{S = targetId.ToString()}}
                },
                ScanIndexForward = true
            };

            var chargesLists = await _amazonDynamoDb.QueryAsync(request).ConfigureAwait(false);

            return chargesLists?.ToConsolidatedChargeDomain();
        }
    }
}
