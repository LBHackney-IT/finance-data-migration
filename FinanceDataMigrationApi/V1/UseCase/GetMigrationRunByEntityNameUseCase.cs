using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetMigrationRunByEntityNameUseCase : IGetMigrationRunByEntityNameUseCase
    {
        private readonly IMigrationRunDynamoGateway _gateway;

        public GetMigrationRunByEntityNameUseCase(IMigrationRunDynamoGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<MigrationRunResponse> ExecuteAsync(string entityName)
        {
            var data = await _gateway.GetMigrationRunByEntityNameAsync(entityName).ConfigureAwait(false);
            return data?.ToResponse();
        }
    }
}
