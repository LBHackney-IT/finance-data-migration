using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    [Table("DMDetailedChargesEntity")]
    public class DmDetailedChargesEntity
    {
        [Key]
        [Column("id")]
        public static Guid Id  => Guid.NewGuid();

        [ForeignKey("id_dynamodb")]
        [Column("charge_id")]
        public Guid ChargeId { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }

        [Column("property_reference")]
        public string PropertyReference { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("sub_type")]
        public string SubType { get; set; }

        [Column("charge_type")]
        public ChargeType? ChargeType { get; set; }

        [Column("frequency")]
        public string Frequency { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("charge_code")]
        public string ChargeCode { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

    }
}
