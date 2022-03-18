using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Boundary.Response.MetaData;
using Hackney.Shared.HousingSearch.Domain.Asset;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAssetGateway
    {
        public Task<int> SaveAssetsIntoSql(string lastHint, XElement xml);
        /*public Task<APIResponse<GetAssetListResponse>> DownloadAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey);*/
        public Task<AssetPaginationResponse> DownloadAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey = null);
        public Task<AssetPaginationResponse> GetAll(Dictionary<string, AttributeValue> lastEvaluatedKey = null);

        public Task<List<Hackney.Shared.Asset.Domain.Asset>> GetAllBySegmentScan();
    }
}
