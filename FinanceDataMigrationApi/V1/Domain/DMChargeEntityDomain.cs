using System;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DMChargeEntityDomain
    {

        public long Id { get; set; }
        public Guid IdDynamodb { get; set; }
        public Guid TargetId { get; set; }
        public string PaymentReference { get; set; }
        public string TargetType { get; set; }
        public string ChargeGroup { get; set; }
        public string DetailedCharges { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string ChargeType { get; set; }
        public string Frequency { get; set; }
        public decimal? Amount { get; set; }
        public string ChargeCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsTransformed { get; set; }
        public bool IsLoaded { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
