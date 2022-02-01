using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAssetGateway
    {
        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml);

        public Task<AssetPaginationResponse> GetAll(int count,
            Dictionary<string, AttributeValue> lastEvaluatedKey = null);
    }
}
