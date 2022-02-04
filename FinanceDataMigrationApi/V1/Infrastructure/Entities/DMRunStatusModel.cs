using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Table("DMRunStatus")]
    public class DmRunStatusModel
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("all_asset_dm_completed")]
        public bool AllAssetDmCompleted { get; set; }
    }
}