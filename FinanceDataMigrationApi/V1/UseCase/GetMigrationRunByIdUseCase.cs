using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetMigrationRunByIdUseCase : IGetMigrationRunByIdUseCase
    {
        private readonly IMigrationRunDynamoGateway _gateway;

        public GetMigrationRunByIdUseCase(IMigrationRunDynamoGateway gateway)
        {
            _gateway = gateway;
        }

        //TODO NM: [LogCall]
        public async Task<MigrationRunResponse> ExecuteAsync(Guid id)
        {
            var data = await _gateway.GetMigrationRunByIdAsync(id).ConfigureAwait(false);
            return data?.ToResponse();
        }
    }
}
