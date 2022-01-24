using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITransactionsDynamoDbGateway
    {
        Task<List<TransactionDbEntity>> GetTenureByTenureId(Guid tenureId);
    }
}
