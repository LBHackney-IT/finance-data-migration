using System;
using System.ComponentModel.DataAnnotations;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DetailedCharges
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string SubType { get; set; }

        public ChargeType ChargeType { get; set; }

        public string ChargeCode { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Range(0, (double) decimal.MaxValue, ErrorMessage = "The amount value is wrong")]
        public decimal Amount { get; set; }

        [RequiredDateTime]
        public DateTime StartDate { get; set; }

        [RequiredDateTime]
        public DateTime EndDate { get; set; }
    }
}
