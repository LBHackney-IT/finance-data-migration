using System.Threading.Tasks;
using System.Xml.Linq;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class TenureSaveToSqlUseCase : ITenureSaveToSqlUseCase
    {
        private readonly ITenureGateway _gateway;

        public TenureSaveToSqlUseCase(ITenureGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<int> ExecuteAsync(TenurePaginationResponse tenurePaginationResponse)
        {
            return await _gateway.SaveTenuresIntoSql(tenurePaginationResponse).ConfigureAwait(false);
        }
    }
}
