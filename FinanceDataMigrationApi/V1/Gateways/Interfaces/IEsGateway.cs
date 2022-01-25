using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Nest;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IEsGateway
    {
        Task<IndexResponse> IndexTransaction(QueryableTransaction transaction);

        Task BulkIndexTransaction(List<QueryableTransaction> transactions);
    }
}
