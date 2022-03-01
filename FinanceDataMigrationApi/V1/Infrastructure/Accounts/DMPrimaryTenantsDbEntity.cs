using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    [Table("DMDynamoTenureHouseHoldMembers")]
    public class DMPrimaryTenantsDbEntity
    {
        [Column("row_id")]
        public long RowId { get; set; }

        [Column("id")]
        public Guid Id { get; set; }

        [Column("fullname")]
        public string FullName { get; set; }

        [Column("is_responsible")]
        public bool IsResponsible { get; set; }

        [Column("tenure_id")]
        public Guid TenureId { get; set; }

        public DmTenureDbEntity TenureDbEntity { get; set; }
    }
}
