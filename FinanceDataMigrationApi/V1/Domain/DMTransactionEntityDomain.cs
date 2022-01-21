using System;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DMTransactionEntityDomain
    {
        public long Id { get; set; }

        public Guid IdDynamodb { get; set; }

        public Guid TargetId { get; set; }

        public string TargetType { get; set; }

        public int PeriodNo { get; set; }

        public short FinancialYear { get; set; }

        public short FinancialMonth { get; set; }

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

        public string Person { get; set; }

        public string Fund { get; set; }

        public bool IsTransformed { get; set; }

        public bool IsLoaded { get; set; }

        public bool IsIndexed { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
