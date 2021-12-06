using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class GetMigrationRunListUseCase : IGetMigrationRunListUseCase
    {
        private readonly IMigrationRunDynamoGateway _gateway;

        public GetMigrationRunListUseCase(IMigrationRunDynamoGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<List<MigrationRun>> ExecuteAsync()
        {
            return await _gateway.GetAllMigrationRunsAsync().ConfigureAwait(false);
        }
    }
}
