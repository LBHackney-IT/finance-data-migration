using FinanceDataMigrationApi.V1.Domain;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface IMigrationRunGateway
    {
        public Task<DMRunLogDomain> GetDMRunLogByEntityNameAsync(string dynamoDbTableName);
        public Task AddAsync(DMRunLogDomain migrationRunDomain);
        public Task UpdateAsync(DMRunLogDomain migrationRunDomain);
    }
}
