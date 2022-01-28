using System.Threading.Tasks;
using System.Xml.Linq;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.UseCase
{
    public class AssetSaveToSqlUseCase : IAssetSaveToSqlUseCase
    {
        private readonly IAssetGateway _gateway;

        public AssetSaveToSqlUseCase(IAssetGateway gateway)
        {
            _gateway = gateway;
        }
        public Task<int> ExecuteAsync(string lastHint, XElement xml)
        {
            return _gateway.SaveAssetsIntoSql(lastHint, xml);
        }
    }
}
