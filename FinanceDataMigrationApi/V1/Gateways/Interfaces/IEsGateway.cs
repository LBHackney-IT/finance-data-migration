using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IEsGateway<in T>
    where T : class
    {
        Task BulkIndex(IEnumerable<T> queryableEntity);
    }
}
