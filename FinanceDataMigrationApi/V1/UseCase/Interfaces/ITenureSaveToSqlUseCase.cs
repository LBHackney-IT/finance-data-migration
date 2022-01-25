using System.Threading.Tasks;
using System.Xml.Linq;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces
{
    public interface ITenureSaveToSqlUseCase
    {
        public Task<int> ExecuteAsync(string lastHint, XElement xml);
    }
}
