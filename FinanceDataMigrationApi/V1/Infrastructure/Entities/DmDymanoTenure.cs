using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities
{
    [XmlRoot("Tenure")]
    [Serializable()]
    [Table("DMDynamoTenure")]
    public class DmDynamoTenure
    {
        [Column("id")]
        [XmlAttribute("id")]
        public Guid Id { get; set; }
        [Column("payment_reference")]
        [XmlAttribute("payment_reference")]
        public string PaymentReference { get; set; }
        [Column("tenure_type_code")]
        [XmlAttribute("tenure_type_code")]
        public string TenureTypeCode { get; set; }
        [Column("tenure_type_desc")]
        [XmlAttribute("tenure_type_desc")]
        public string TenureTypeDesc { get; set; }
        [Column("tenured_asset_full_address")]
        [XmlAttribute("tenured_asset_full_address")]
        public string TenuredAssetFullAddress { get; set; }
        [Column("terminated_reason_code")]
        [XmlAttribute("terminated_reason_code")]
        public string TerminatedReasonCode { get; set; }
        [Column("timex")]
        [XmlAttribute("timex")]
        public DateTime Timex { get; set; }

        [XmlAttribute("HouseHoldMembers")]
        public List<DmDynamoTenureHouseHoldMembers> DynamoHouseHoldMembers { get; set; }
    }
}
