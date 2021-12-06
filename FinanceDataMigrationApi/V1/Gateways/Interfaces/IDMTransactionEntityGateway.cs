using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMTransactionEntityGateway
    {
        Task<DMTransactionEntity> GetDMTransactionEntityByIdAsync(int id);

        Task<IList<DMTransactionEntity>> ListAsync();

        //public Task LoadDMTransactionEntities();
        Task <int> ExtractAsync(DateTime? processingDate);

        public Task<int> UpdateDMTransactionEntityItems(IList<DMTransactionEntity> items);

        public Task UpdateDMTransactionEntities(DMTransactionEntity dMTransactionEntity);
    }
}
