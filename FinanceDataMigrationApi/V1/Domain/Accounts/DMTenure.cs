using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;

namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class DmTenure
    {
        public Guid Id { get; set; }

        public string PaymentReference { get; set; }

        public string TenureTypeCode { get; set; }

        public string TenureTypeDesc { get; set; }

        public string FullAddress { get; set; }

        public string TerminatedReasonCode { get; set; }

        public DateTime Timex { get; set; }

        public List<DmPrimaryTenants> PrimaryTenants { get; set; }
    }
}
