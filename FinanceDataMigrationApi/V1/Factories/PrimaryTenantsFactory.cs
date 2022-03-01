using FinanceDataMigrationApi.V1.Domain.Accounts;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class PrimaryTenantsFactory
    {
        public static DmPrimaryTenants ToDomain(this DMPrimaryTenantsDbEntity dbEntity)
        {
            return new DmPrimaryTenants
            {
                FullName = dbEntity.FullName,
                Id = dbEntity.Id,
                IsResponsible = dbEntity.IsResponsible,
                RowId = dbEntity.RowId,
                TenureId = dbEntity.TenureId
            };
        }
    }
}
