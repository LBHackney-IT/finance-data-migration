using System;

namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class DmPrimaryTenants
    {
        public long RowId { get; set; }

        public Guid Id { get; set; }

        public string FullName { get; set; }

        public bool IsResponsible { get; set; }

        public Guid TenureId { get; set; }

    }
}
