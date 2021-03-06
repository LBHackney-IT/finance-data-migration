using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Asset
{
    public interface IAssetSaveToSqlUseCase
    {
        public Task<int> ExecuteAsync(string lastHint, XElement xml);
    }
}
