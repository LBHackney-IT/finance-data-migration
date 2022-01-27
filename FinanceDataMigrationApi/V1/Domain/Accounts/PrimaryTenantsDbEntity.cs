using System;

namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class PrimaryTenantsDbEntity
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
    }
}
