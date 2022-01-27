using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMChargesEntity")]
    public class ChargesDbEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_dynamodb")]
        public Guid IdDynamoDb { get; set; }

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
        public List<DetailedCharges> DetailedCharges { get; set; }

        [Column("migration_status")]
        public EMigrationStatus MigrationStatus { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
