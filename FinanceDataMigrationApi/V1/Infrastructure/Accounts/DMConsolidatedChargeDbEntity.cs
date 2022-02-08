using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Accounts
{
    [Table("DMAccountConsolidatedCharge")]
    public class DmConsolidatedChargeDbEntity
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("payment_reference")]
        public string PaymentReference { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("frequency")]
        public string Frequency { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }

        public DmAccountDbEntity AccountDbEntity { get; set; }
    }
}
