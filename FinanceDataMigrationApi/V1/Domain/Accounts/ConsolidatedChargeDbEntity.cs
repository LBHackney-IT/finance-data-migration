namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class ConsolidatedChargeDbEntity
    {
        public string Type { get; set; }

        public string Frequency { get; set; }

        public decimal Amount { get; set; }
    }
}
