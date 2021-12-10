using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    /// <summary>
    /// The Data Migration Transaction Entity.
    /// </summary>
    [Table("DMTransactionEntity")]
    public class DMTransactionEntity
    {
        [Key]
        [Column("id")]
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

        [Column("bank_account_number")]
        public string BankAccountNumber { get; set; }

        [Column("is_suspense")]
        public bool IsSuspense { get; set; }

        [Column("suspense_resolution_info")]
        public string SuspenseResolutionInfo { get; set; }

        [Column("paid_amount")]
        public decimal PaidAmount { get; set; }

        [Column("charged_amount")]
        public decimal ChargedAmount { get; set; }

        [Column("housing_benefit_amount")]
        public decimal HousingBenefitAmount { get; set; }

        [Column("balance_amount")]
        public decimal BalanceAmount { get; set; }

        [Column("person")]
        public string Person { get; set; }

        [Column("fund")]
        public string Fund { get; set; }

        [Column("is_transformed")]
        public bool IsTransformed { get; set; }

        [Column("is_loaded")]
        public bool IsLoaded { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

    }
}
