using Amazon.DynamoDBv2.Model;
using Hackney.Shared.HousingSearch.Domain.Asset;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Boundary.Response
{
    public class AssetPaginationResponse
    {
        public Dictionary<string, AttributeValue> LastKey { get; set; }
        public List<Asset> Assets { get; set; }
    }
}
