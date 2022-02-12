using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.Infrastructure.Enums;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;

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
            var chargeModel = new Dictionary<string, AttributeValue>();


            chargeModel.PureAdd("id", new AttributeValue { S = charge.IdDynamodb.ToString() });
            chargeModel.PureAdd("target_id", new AttributeValue { S = charge.TargetId.ToString() });
            chargeModel.PureAdd("target_type", new AttributeValue { S = charge.TargetType.ToString().Trim() });
            chargeModel.PureAdd("charge_group", new AttributeValue { S = charge.ChargeGroup.ToString().Trim() });
            chargeModel.PureAdd("charge_year", new AttributeValue { N = charge.DetailedCharges.FirstOrDefault()?.StartDate.Year.ToString().Trim() });

            ChargeSubGroup subGroup = charge.DetailedCharges.FirstOrDefault()?.StartDate.Year >= DateTime.Now.Year - 1
                ? ChargeSubGroup.Estimate
                : ChargeSubGroup.Actual;

            chargeModel.PureAdd("charge_sub_group", new AttributeValue { S = subGroup.ToString() });
            chargeModel.PureAdd("created_at", new AttributeValue { S = DateTime.Today.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            chargeModel.PureAdd("created_by", new AttributeValue { S = "Migration" });

            if (charge.DetailedCharges != null && charge.DetailedCharges.Count > 0)
            {
                var chargeDetailedChargesModelList = new List<AttributeValue>();
                foreach (DmDetailedCharges p in charge.DetailedCharges)
                {
                    var chargeDetailedChargesModel = new Dictionary<string, AttributeValue>();
                    chargeDetailedChargesModel.PureAdd("chargeCode", new AttributeValue { S = p.ChargeCode });
                    chargeDetailedChargesModel.PureAdd("frequency", new AttributeValue { S = p.Frequency });
                    chargeDetailedChargesModel.PureAdd("amount", new AttributeValue { N = p.Amount.ToString("F") });
                    chargeDetailedChargesModel.PureAdd("endDate", new AttributeValue { S = p.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
                    chargeDetailedChargesModel.PureAdd("chargeType", new AttributeValue { S = p.ChargeType?.ToString() ?? "NA" });
                    chargeDetailedChargesModel.PureAdd("subType", new AttributeValue { S = p.SubType?.ToString().Trim() });
                    chargeDetailedChargesModel.PureAdd("type", new AttributeValue { S = p.Type?.ToString() });
                    chargeDetailedChargesModel.PureAdd("startDate", new AttributeValue { S = p.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
                    chargeDetailedChargesModelList.Add(new AttributeValue
                    {
                        M = chargeDetailedChargesModel
                    });
                }

                chargeModel.PureAdd("detailed_charges", new AttributeValue { L = chargeDetailedChargesModelList });
            }

            return chargeModel;
            /*return new Dictionary<string, AttributeValue>()
            {
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
            };*/
        }

    }
}
