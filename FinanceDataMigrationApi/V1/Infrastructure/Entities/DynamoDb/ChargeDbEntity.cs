using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Infrastructure.Entities.DynamoDb
{
    [DynamoDBTable("Charges", LowerCamelCaseProperties = true)]
    public class ChargeDbEntity
    {
        [DynamoDBHashKey(AttributeName = "target_id")]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey(AttributeName = "id")]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "target_type", Converter = typeof(DynamoDbEnumConverter<ChargeTargetType>))]
        public ChargeTargetType TargetType { get; set; }

        [DynamoDBProperty(AttributeName = "charge_group", Converter = typeof(DynamoDbEnumConverter<ChargeGroup>))]
        public ChargeGroup ChargeGroup { get; set; }

        /// <summary>
        /// Required only for ChargeGroup = Leaseholders
        /// </summary>
        [DynamoDBProperty(AttributeName = "charge_sub_group", Converter = typeof(DynamoDbEnumConverter<ChargeSubGroup>))]
        public ChargeSubGroup? ChargeSubGroup { get; set; }

        [DynamoDBProperty(AttributeName = "charge_year")]
        public short ChargeYear { get; set; }

        [DynamoDBProperty(AttributeName = "detailed_charges", Converter = (typeof(DynamoDbObjectListConverter<DmDetailedCharges>)))]
        public IEnumerable<DmDetailedCharges> DetailedCharges { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_at", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime? LastUpdatedAt { get; set; }
    }
}
