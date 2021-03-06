using System;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DmTransaction
    {
        public long Id { get; set; }

        public Guid IdDynamodb { get; set; }

        public Guid TargetId { get; set; }

        public string TargetType { get; set; }

        public decimal PeriodNo { get; set; }

        public int FinancialYear { get; set; }

        public int FinancialMonth { get; set; }

        public string TransactionSource { get; set; }

        public string TransactionType { get; set; }

        public DateTime TransactionDate { get; set; }

        public decimal TransactionAmount { get; set; }

        public string PaymentReference { get; set; }

        public string BankAccountNumber { get; set; }

        public bool IsSuspense { get; set; }

        public string SuspenseResolutionInfo { get; set; }

        public decimal? PaidAmount { get; set; }

        public decimal? ChargedAmount { get; set; }

        public decimal? HousingBenefitAmount { get; set; }

        public decimal? BalanceAmount { get; set; }

        public string Sender { get; set; }

        public string Fund { get; set; }

        public string Address { get; set; }

        public EMigrationStatus MigrationStatus { get; set; }

        public bool IsIndexed { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public string LastUpdatedBy { get; set; }
    }
}
