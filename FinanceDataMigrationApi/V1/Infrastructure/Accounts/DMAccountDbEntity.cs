using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    /// <summary>
    /// The Data Migration Account Entity.
    /// </summary>
    [Table("DMAccountsEntity")]
    public class DmAccountDbEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("id_dynamodb")]
        public Guid DynamoDbId { get; set; }

        [Column("target_id")]
        public Guid? TargetId { get; set; }

        [Column("target_type")]
        public string TargetType { get; set; }

        [Column("account_type")]
        public string AccountType { get; set; }

        [Column("rent_group_type")]
        public string RentGroupType { get; set; }

        [Column("agreement_type")]
        public string AgreementType { get; set; }

        [Column("account_balance")]
        public decimal? AccountBalance { get; set; }

        [Column("consolidated_balance")]
        public decimal? ConsolidatedBalance { get; set; }

        [Column("parent_account_id")]
        public Guid? ParentAccountId { get; set; }

        [Column("payment_reference")]
        public string PaymentReference { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        //[Column("payment_by")]
        //public string CreatedBy { get; set; }

        //[Column("last_updated_at")]
        //public DateTime LastUpdatedAt { get; set; }

        //[Column("last_updated_by")]
        //public string LastUpdatedBy { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("end_reason_code")]
        public string EndReasonCode { get; set; }


        [Column("account_status")]
        public string AccountStatus { get; set; }

        [Column("migration_status")]
        public EMigrationStatus MigrationStatus { get; set; }

        [Column("is_indexed")]
        public bool? IsIndexed { get; set; }

        [Column("tenure")]
        public DmTenureDbEntity Tenure { get; set; }

        [Column("consolidated_charges")]
        public List<DmConsolidatedChargeDbEntity> ConsolidatedCharges { get; set; }
    }
}
