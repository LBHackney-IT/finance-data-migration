using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class TenureDbEntity
    {
        public string TenureId { get; set; }

        public TenureTypeDbEntity TenureType { get; set; }

        public string FullAddress { get; set; }

        public List<PrimaryTenantsDbEntity> PrimaryTenants { get; set; }
    }
}
