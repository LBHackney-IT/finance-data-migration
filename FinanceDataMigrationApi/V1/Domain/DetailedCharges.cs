using System;
using System.ComponentModel.DataAnnotations;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.Domain
{
    public class DetailedCharges
    {
        public decimal Id { get; set; }
        public long ChargeId { get; set; }
        public string PropertyReference { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string ChargeType { get; set; }
        public string Frequency { get; set; }
        public decimal Amount { get; set; }
        public string ChargeCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
