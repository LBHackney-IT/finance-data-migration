using System;
using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class Charge
    {
        public long Id { get; set; }
        public Guid IdDynamodb { get; set; }
        public Guid TargetId { get; set; }
        public string PaymentReference { get; set; }
        public string PropertyReference { get; set; }
        public string TargetType { get; set; }
        public string ChargeGroup { get; set; }
        public List<DetailedCharges> DetailedCharges { get; set; }
        public EMigrationStatus MigrationStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int ChargeYear { get; set; }
    }
}
