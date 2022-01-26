using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    /// <summary>
    /// The Data Migration Tenure Entity.
    /// </summary>
    [Table("DMDynamoTenure")]
    public class DMTenureEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("household_members")]
        public string HouseholdMembers { get; set; }

        [Column("tenure_type")]
        public string TenureType { get; set; }

        [Column("tenured_asset")]
        public string TenuredAsset { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }
    }
}
