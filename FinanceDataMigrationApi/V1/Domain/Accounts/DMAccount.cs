using System;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;

namespace FinanceDataMigrationApi.V1.Domain.Accounts
{
    public class DmAccount
    {
        public long Id { get; set; }

        public Guid DynamoDbId { get; set; }

        public Guid? TargetId { get; set; }

        public string TargetType { get; set; }

        public string AccountType { get; set; }

        public string RentGroupType { get; set; }

        public string AgreementType { get; set; }

        public decimal? AccountBalance { get; set; }

        public decimal? ConsolidatedBalance { get; set; }

        public Guid? ParentAccountId { get; set; }

        public string PaymentReference { get; set; }

        //[Column("created_at")]
        //public DateTime CreatedAt { get; set; }

        //[Column("payment_by")]
        //public string CreatedBy { get; set; }

        //[Column("last_updated_at")]
        //public DateTime LastUpdatedAt { get; set; }

        //[Column("last_updated_by")]
        //public string LastUpdatedBy { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string EndReasonCode { get; set; }

        public string ConsolidatedCharges { get; set; }

        public string Tenure { get; set; }

        public string AccountStatus { get; set; }

        public EMigrationStatus MigrationStatus { get; set; }
    }
}
