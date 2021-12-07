using FinanceDataMigrationApi.V1.Domain;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IDMRunLogGateway
    {
        public Task<DMRunLogDomain> GetDMRunLogByEntityNameAsync(string dynamoDbTableName);
        public Task<DMRunLogDomain> AddAsync(DMRunLogDomain migrationRunDomain);
        public Task<bool> UpdateAsync(DMRunLogDomain dmRunLogDomain);
    }
}
