using Hackney.Shared.Tenure.Infrastructure;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public interface ITenureDynamoDbGateway
    {
        Task<TenureInformationDb> GetTenureById(Guid tenureId);
    }
}
