using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static partial class ChargeFactory
    {

        public static DmCharge ToDomain(this DmChargesDbEntity dMChargesEntity)
        {
            return dMChargesEntity == null
                ? null
                : new DmCharge()
                {
                    Id = dMChargesEntity.Id,
                    IdDynamodb = dMChargesEntity.IdDynamoDb,
                    TargetId = dMChargesEntity.TargetId ?? Guid.Empty,
                    PaymentReference = dMChargesEntity.PaymentReference,
                    PropertyReference = dMChargesEntity.PropertyReference,
                    TargetType = dMChargesEntity.TargetType,
                    ChargeGroup = dMChargesEntity.ChargeGroup,
                    DetailedCharges = dMChargesEntity.DetailedChargesDbEntities?
                        .Select(p => new DmDetailedCharges
                        {
                            Amount = p.Amount,
                            ChargeCode = p.ChargeCode,
                            ChargeId = p.ChargeId,
                            ChargeType = p.ChargeType,
                            EndDate = p.EndDate,
                            Frequency = p.Frequency,
                            Id = p.Id,
                            StartDate = p.StartDate,
                            PropertyReference = p.PropertyReference,
                            SubType = p.SubType,
                            Type = p.Type
                        }).ToList(),
                    MigrationStatus = dMChargesEntity.MigrationStatus,
                    CreatedAt = dMChargesEntity.CreatedAt
                };
        }

        public static List<DmCharge> ToDomain(this IList<DmChargesDbEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }
        public static Dictionary<string, AttributeValue> ToQueryRequest(this DmCharge charge)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = charge.IdDynamodb.ToString()}},
                {"target_id", new AttributeValue {S = charge.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = charge.TargetType.ToString()}},
                {"charge_group", new AttributeValue {S = charge.ChargeGroup.ToString()}},
                {"charge_year", new AttributeValue {N = charge.DetailedCharges.FirstOrDefault()?.StartDate.Year.ToString()}},
                {
                    "detailed_charges",charge.DetailedCharges==null?new AttributeValue(""):
                    new AttributeValue
                    {
                        L=
                            charge.DetailedCharges.Select(p=>
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"chargeCode", new AttributeValue {S = p.ChargeCode??""}},
                                    {"frequency", new AttributeValue {S = p.Frequency??""}},
                                    {"amount", new AttributeValue {N = p.Amount.ToString("F")}},
                                    {"endDate", new AttributeValue {S =p.EndDate==null?"": p.EndDate.ToString("F")}},
                                    {"chargeType", new AttributeValue {S = p.ChargeType?.ToString()??""}},
                                    {"subType", new AttributeValue {S = p.SubType?.ToString()??""}},
                                    {"type", new AttributeValue {S = p.Type?.ToString()??""}},
                                    {"startDate", new AttributeValue {S =p.StartDate==null?"":p.StartDate.ToString("F")}},
                                }
                            }).ToList()
                    }
                }
            };
        }

    }
}
