using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.Tests.V1.Gateways
{
    public class DmRunLogGatewayProxy : DMRunLogGateway, IDMRunLogGateway
    {
        public bool GetDmRunLogByEntityNameHasBeenCalled { get; private set; }
        public Task<DMRunLogDomain> DmRunLogDomainResult { get; private set; }

        public DmRunLogGatewayProxy(DatabaseContext context) : base(context)
        {
        }

        public new Task<DMRunLogDomain> GetDMRunLogByEntityNameAsync(string dynamoDbTableName)
        {
            DmRunLogDomainResult =  base.GetDMRunLogByEntityNameAsync(dynamoDbTableName);
            GetDmRunLogByEntityNameHasBeenCalled = true;
            return DmRunLogDomainResult;
        }
    }
}
