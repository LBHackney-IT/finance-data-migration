using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IEsGateway
    {
        Task<IndexResponse> IndexTransaction(QueryableTransaction transaction);

        Task BulkIndexTransaction(List<QueryableTransaction> transactions);

        Task BulkIndexAccounts(List<QueryableAccount> accounts);

        Task<Hackney.Shared.HousingSearch.Gateways.Models.Tenures.QueryableTenure> GetTenureByPrn(string prn);
    }
}
