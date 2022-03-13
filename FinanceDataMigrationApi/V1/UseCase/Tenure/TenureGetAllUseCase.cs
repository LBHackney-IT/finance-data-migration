using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Tenure;

namespace FinanceDataMigrationApi.V1.UseCase.Tenure
{
    public class TenureGetAllUseCase : ITenureGetAllUseCase
    {
        private readonly ITenureGateway _tenureGateway;

        public TenureGetAllUseCase(ITenureGateway tenureGateway)
        {
            _tenureGateway = tenureGateway;
        }
        public async Task<TenurePaginationResponse> ExecuteAsync(Dictionary<string, AttributeValue> lastEvaluatedKey = null)
        {
            return await _tenureGateway.GetAll(lastEvaluatedKey).ConfigureAwait(false);
        }
    }
}
