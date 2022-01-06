using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    [Table("DMChargeEntity")]
    public class DMChargeEntity
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

        [Column("target_type")]
        public string TargetType { get; set; }

        [Column("charge_group")]
        public string ChargeGroup { get; set; }

        [Column("detailed_charges")]
        public string DetailedCharges { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("sub_type")]
        public string SubType { get; set; }

        [Column("charge_type")]
        public string ChargeType { get; set; }

        [Column("frequency")]
        public string Frequency { get; set; }

        [Column("amount")]
        public decimal? Amount { get; set; }

        [Column("charge_code")]
        public string ChargeCode { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_transformed")]
        public bool IsTransformed { get; set; }

        [Column("is_loaded")]
        public bool IsLoaded { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
