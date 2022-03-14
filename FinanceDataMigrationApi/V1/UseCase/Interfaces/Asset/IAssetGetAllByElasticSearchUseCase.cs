using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset
{
    public interface IAssetGetAllByElasticSearchUseCase
    {
        public Task<AssetPaginationResponse> ExecuteAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey);
    }
}
