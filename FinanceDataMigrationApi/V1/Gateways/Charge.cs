using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class Charge
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public TargetType TargetType { get; set; }
        public ChargeGroup ChargeGroup { get; set; }
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
