using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMDetailedCharges")]
    public class DetailedChargesDbEntity
    {
        [Key]
        [Column("id")]
        public decimal Id { get; set; }
        [ForeignKey("id_dynamodb")]
        [Column("charge_id")]
        public Guid ChargeId { get; set; }
        [Column("property_reference")]
        public string PropertyReference { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("sub_type")]
        public string SubType { get; set; }
        [Column("charge_type")]
        public string ChargeType { get; set; }
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
