using System.Threading.Tasks;
using System.Xml.Linq;
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
        public async Task<int> ExecuteAsync(string lastHint, XElement xml)
        {
            return await _gateway.SaveTenuresIntoSql(lastHint, xml).ConfigureAwait(false);
        }
    }
}
