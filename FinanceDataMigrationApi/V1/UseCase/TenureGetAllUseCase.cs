using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class TenureGetAllUseCase: ITenureGetAllUseCase
    {
        private readonly ITenureGateway _tenureGateway;

        public TenureGetAllUseCase(ITenureGateway tenureGateway)
        {
            _tenureGateway = tenureGateway;
        }
        public Task<TenurePaginationResponse> ExecuteAsync(string paginationToken = "{}")
        {
            return _tenureGateway.GetAll(paginationToken);
        }
    }
}
