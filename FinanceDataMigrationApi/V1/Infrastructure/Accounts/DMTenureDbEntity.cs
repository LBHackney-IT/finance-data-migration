using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    [Table("DMDynamoTenure")]
    public class DmTenureDbEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }

        [Column("tenure_type_code")]
        public string TenureTypeCode { get; set; }

        [Column("tenure_type_desc")]
        public string TenureTypeDesc { get; set; }

        [Column("tenured_asset_full_address")]
        public string FullAddress { get; set; }

        [Column("terminated_reason_code")]
        public string TerminatedReasonCode { get; set; }

        [Column("timex")]
        public DateTime Timex { get; set; }

        public List<DMPrimaryTenantsDbEntity> PrimaryTenants { get; set; }
        public DmAccountDbEntity AccountDbEntity { get; set; }
    }
}
