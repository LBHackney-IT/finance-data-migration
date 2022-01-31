using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [Serializable()]
    [Table("DMDynamoTenureHouseHoldMembers")]
    public class DmDynamoTenureHouseHoldMembers
    {
        [Column("row_id")]
        [Key]
        public long RowId { get; set; }
        [Column("id")]
        [XmlAttribute("id")]
        public Guid Id { get; set; }
        [Column("fullname")]
        [XmlAttribute("fullname")]
        public string Fullname { get; set; }
        [Column("is_responsible")]
        [XmlAttribute("is_responsible")]
        public bool IsResponsible { get; set; }

        [Column("tenure_id")]
        [XmlAttribute("tenure_id")]
        public Guid TenureId { get; set; }

        public DmDynamoTenure DynamoTenure { get; set; }
    }
}
