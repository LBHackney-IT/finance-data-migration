using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMRunStatus")]
    public class DmRunStatusModel
    {
        [Column("id")]
        [Key]
        public int Id { get; set; } = 1;
        [Column("all_asset_dm_completed")]
        public bool AllAssetDmCompleted { get; set; }
        [Column("all_tenure_dm_completed")]
        public bool AllTenureDmCompleted { get; set; }
        [Column("charge_extract_date")]
        public DateTime ChargeExtractDate { get; set; }
        [Column("charge_load_date")]
        public DateTime ChargeLoadDate { get; set; }
    }
}
