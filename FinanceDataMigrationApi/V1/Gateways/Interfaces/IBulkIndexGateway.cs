using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IBulkIndexGateway<T> where T : class
    {
        public Task<Task> IndexAllAsync(List<T> queryableList);
    }
}
