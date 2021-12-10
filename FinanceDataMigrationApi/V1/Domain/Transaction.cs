using FinanceDataMigrationApi.V1.Infrastructure;
using FinancialTransactionsApi.V1.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class Transaction
    {

        [NotNull]
        public Guid TargetId { get; set; }

        public TargetType TargetType { get; set; }
        [Required]
        public short PeriodNo { get; set; }
        [Required]
        public short FinancialYear { get; set; }
        [Required]
        public short FinancialMonth { get; set; }
        [Required]
        public string TransactionSource { get; set; }
        [AllowedValues(typeof(TransactionType))]
        public TransactionType TransactionType { get; set; }
        [RequiredDateTime]
        public DateTime TransactionDate { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TransactionAmount { get; set; }
        [Required]
        public string PaymentReference { get; set; }
        [AllowNull]
        public string BankAccountNumber { get; set; }
        [Required]
        public bool IsSuspense { get; set; }
        [AllowNull]
        public SuspenseResolutionInfo SuspenseResolutionInfo { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal PaidAmount { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ChargedAmount { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal BalanceAmount { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal HousingBenefitAmount { get; set; }

        public string Address { get; set; }
        [Required]
        public TransactionPerson Person { get; set; }
        [Required]
        public string Fund { get; set; }
    }
}