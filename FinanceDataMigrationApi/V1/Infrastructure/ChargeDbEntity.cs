using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Domain;
using Hackney.Core.DynamoDb.Converters;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    [DynamoDBTable("Charges", LowerCamelCaseProperties = true)]
    public class ChargeDbEntity
    {
        [DynamoDBHashKey(AttributeName = "target_id")]
        public Guid TargetId { get; set; }

        [DynamoDBRangeKey(AttributeName = "id")]
        public Guid Id { get; set; }

        [DynamoDBProperty(AttributeName = "target_type"/*, Converter = typeof(DynamoDbEnumConverter<TargetType>)*/)]
        public string TargetType { get; set; }

        [DynamoDBProperty(AttributeName = "charge_group"/*, Converter = typeof(DynamoDbEnumConverter<ChargeGroup>)*/)]
        public string ChargeGroup { get; set; }

        [DynamoDBProperty(AttributeName = "detailed_charges", Converter = (typeof(DynamoDbObjectListConverter<DetailedCharges>)))]
        public IEnumerable<DetailedCharges> DetailedCharges { get; set; }

        [DynamoDBProperty(AttributeName = "created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_by")]
        public string LastUpdatedBy { get; set; }

        [DynamoDBProperty(AttributeName = "created_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty(AttributeName = "last_updated_date", Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime LastUpdatedDate { get; set; }
    }
}
