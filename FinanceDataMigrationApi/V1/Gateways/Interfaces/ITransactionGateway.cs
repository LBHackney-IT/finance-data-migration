using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionGateway
    {
        Task<int> ExtractAsync(DateTimeOffset? processingDate);
        Task<IList<DmTransaction>> GetTransformedListAsync(int count);
        Task<IList<DmTransaction>> GetExtractedListAsync(int count);
        Task BatchInsert(List<DmTransaction> transactions);

    }
}
