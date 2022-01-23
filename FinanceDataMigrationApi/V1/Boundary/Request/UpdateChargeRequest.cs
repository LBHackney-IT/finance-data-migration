using System;
using System.Collections.Generic;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinancialTransactionsApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Boundary.Request
{
    public class UpdateChargeRequest
    {
        [NonEmptyGuid]
        public Guid Id { get; set; }

        [NonEmptyGuid]
        public Guid TargetId { get; set; }

        [AllowedValues(typeof(TargetType))]
        public TargetType TargetType { get; set; }

        [AllowedValues(typeof(ChargeGroup))]
        public ChargeGroup ChargeGroup { get; set; }

        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }
    }
}
