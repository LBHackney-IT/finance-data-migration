using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset
{
    public interface IAssetGetAllByScanUseCase
    {
        public Task<AssetPaginationResponse> ExecuteAsync(Dictionary<string, AttributeValue> lastEvaluatedKey);
    }
}
