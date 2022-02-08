namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class DmConsolidatedCharge
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string PaymentReference { get; set; }
        public string Type { get; set; }
        public string Frequency { get; set; }
        public decimal Amount { get; set; }
    }
}
