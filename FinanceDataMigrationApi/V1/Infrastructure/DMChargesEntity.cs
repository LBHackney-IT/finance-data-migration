using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    [Table("DMChargesEntity")]
    public class DMChargesEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_dynamodb")]
        public Guid IdDynamodb { get; set; }

        [Column("target_id")]
        public Guid? TargetId { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }

        [Column("property_reference")]
        public string PropertyReference { get; set; }

        [Column("target_type")]
        public string TargetType { get; set; }

        [Column("charge_group")]
        public string ChargeGroup { get; set; }

        [Column("detailed_charges")]
        public string DetailedCharges { get; set; }

        [Column("is_transformed")]
        public bool IsTransformed { get; set; }

        [Column("is_loaded")]
        public bool IsLoaded { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
