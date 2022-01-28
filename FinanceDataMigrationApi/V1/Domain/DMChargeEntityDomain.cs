using System;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DMChargeEntityDomain
    {
        public long Id { get; set; }
        public Guid IdDynamodb { get; set; }
        public Guid TargetId { get; set; }
        public string PaymentReference { get; set; }
        public string PropertyReference { get; set; }
        public string TargetType { get; set; }
        public string ChargeGroup { get; set; }
        public string DetailedCharges { get; set; }
        public bool IsTransformed { get; set; }
        public bool IsLoaded { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
