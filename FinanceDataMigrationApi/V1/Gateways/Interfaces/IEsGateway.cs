using Nest;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IEsGateway<in T> where T : class
    {
        /*Task<IndexResponse> IndexTransaction(QueryableTransaction transaction);

        Task BulkIndexTransaction(List<QueryableTransaction> transactions);

        Task BulkIndexAccounts(List<QueryableAccount> accounts);*/

        Task<IndexResponse> IndexAsync(T queryAbleEntity);

    }
}
