using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IIndexFromIFStoFFSGateway<T> where T : class
    {
        public Task IndexAsync(T queryableObject);
    }
}
