using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMTransactionEntity")]
    public class DmTransactionDbEntity
    {
        [Column("id")]
        [Key]
        public long Id { get; set; }
        [Column("id_dynamodb")]
        public Guid IdDynamodb { get; set; }
        [Column("target_id")]
        public Guid TargetId { get; set; }
        [Column("target_type")]
        public string TargetType { get; set; }
        [Column("period_no")]
        public decimal PeriodNo { get; set; }
        [Column("financial_year")]
        public int FinancialYear { get; set; }
        [Column("financial_month")]
        public int FinancialMonth { get; set; }
        [Column("transaction_source")]
        public string TransactionSource { get; set; }
        [Column("transaction_type")]
        public string TransactionType { get; set; }
        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }
        [Column("transaction_amount")]
        public decimal TransactionAmount { get; set; }
        [Column("payment_reference")]
        public string PaymentReference { get; set; }
        [Column("sort_code")]
        public string SortCode { get; set; }
        [Column("bank_account_number")]
        public string BankAccountNumber { get; set; }
        [Column("is_suspense")]
        public bool IsSuspense { get; set; }
        [Column("suspense_resolution_info")]
        public string SuspenseResolutionInfo { get; set; }
        [Column("paid_amount")]
        public decimal? PaidAmount { get; set; }
        [Column("charged_amount")]
        public decimal? ChargedAmount { get; set; }
        [Column("housing_benefit_amount")]
        public decimal? HousingBenefitAmount { get; set; }
        [Column("balance_amount")]
        public decimal? BalanceAmount { get; set; }
        [Column("sender")]
        public string Sender { get; set; }
        [Column("fund")]
        public string Fund { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("migration_status")]
        public EMigrationStatus MigrationStatus { get; set; }
        [Column("is_indexed")]
        public bool IsIndexed { get; set; }
        [Column("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [Column("created_by")]
        public string CreatedBy { get; set; }
        [Column("last_updated_at")]
        public DateTimeOffset? LastUpdatedAt { get; set; }
        [Column("last_updated_by")]
        public string LastUpdatedBy { get; set; }
    }
}
