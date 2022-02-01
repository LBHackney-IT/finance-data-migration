using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IAccountsDynamoDbGateway
    {
        Task<bool> BatchInsert(List<DMAccountEntity> accounts);
    }
}
