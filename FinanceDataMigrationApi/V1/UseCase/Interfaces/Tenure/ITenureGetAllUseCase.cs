using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Boundary.Response;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Tenure
{
    public interface ITenureGetAllUseCase
    {
        public Task<TenurePaginationResponse> ExecuteAsync(int count, Dictionary<string, AttributeValue> lastEvaluatedKey);
    }
}
