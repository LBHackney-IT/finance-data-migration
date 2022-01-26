using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static partial class ChargeFactory
    {

        public static ChargeDbEntity ToDatabase(this Charge charge)
        {
            if (charge == null)
            {
                return null;
            }

            return new ChargeDbEntity
            {
                Id = charge.Id,
                TargetId = charge.TargetId,
                TargetType = charge.TargetType,
                ChargeGroup = charge.ChargeGroup,
                DetailedCharges = charge.DetailedCharges
            };
        }
        public static DMChargesEntity ToDatabase(this DMChargeEntityDomain dMChargeEntityDomain)
        {
            return dMChargeEntityDomain == null
                ? null
                : new DMChargesEntity()
                {
                    Id = dMChargeEntityDomain.Id,
                    IdDynamodb = dMChargeEntityDomain.IdDynamodb,
                    TargetId = dMChargeEntityDomain.TargetId,
                    PaymentReference = dMChargeEntityDomain.PaymentReference,
                    PropertyReference = dMChargeEntityDomain.PropertyReference,
                    TargetType = dMChargeEntityDomain.TargetType,
                    ChargeGroup = dMChargeEntityDomain.ChargeGroup,
                    DetailedCharges = dMChargeEntityDomain.DetailedCharges,
                    IsTransformed = dMChargeEntityDomain.IsTransformed,
                    IsLoaded = dMChargeEntityDomain.IsLoaded,
                    CreatedAt = dMChargeEntityDomain.CreatedAt
                };
        }

        public static DMChargeEntityDomain ToDomain(this DMChargesEntity dMChargesEntity)
        {
            return dMChargesEntity == null
                ? null
                : new DMChargeEntityDomain()
                {
                    Id = dMChargesEntity.Id,
                    IdDynamodb = dMChargesEntity.IdDynamodb,
                    TargetId = dMChargesEntity.TargetId ?? Guid.Empty,
                    PaymentReference = dMChargesEntity.PaymentReference,
                    PropertyReference = dMChargesEntity.PropertyReference,
                    TargetType = dMChargesEntity.TargetType,
                    ChargeGroup = dMChargesEntity.ChargeGroup,
                    DetailedCharges = dMChargesEntity.DetailedCharges,
                    IsTransformed = dMChargesEntity.IsTransformed,
                    IsLoaded = dMChargesEntity.IsLoaded,
                    CreatedAt = dMChargesEntity.CreatedAt
                };
        }

        public static List<DMChargeEntityDomain> ToDomain(this IList<DMChargesEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }

        public static List<DMChargesEntity> ToDatabase(this IList<DMChargeEntityDomain> dMChargeEntityDomainItems)
        {
            return dMChargeEntityDomainItems.Select(p => p.ToDatabase()).ToList();
        }

        public static Charge ToAddChargeRequest(this DMChargeEntityDomain dmChargesEntityDomain)
        {

            return new Charge()
            {
                Id = dmChargesEntityDomain.IdDynamodb,
                TargetId = dmChargesEntityDomain.TargetId,
                ChargeGroup = JsonSerializer.Deserialize<ChargeGroup>(dmChargesEntityDomain.ChargeGroup),
                DetailedCharges =
                    JsonSerializer.Deserialize<List<DetailedCharges>>(dmChargesEntityDomain.DetailedCharges),
                TargetType = JsonSerializer.Deserialize<TargetType>(dmChargesEntityDomain.TargetType)
            };
        }

        public static List<Charge> ToAddChargeRequestList(this IList<DMChargeEntityDomain> dmChargesEntityDomains)
        {
            return dmChargesEntityDomains.Select(item => item.ToAddChargeRequest()).ToList();
        }

        public static Dictionary<string, AttributeValue> ToQueryRequest(this Charge charge)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = charge.Id.ToString()}},
                {"target_id", new AttributeValue {S = charge.TargetId.ToString()}},
                {"target_type", new AttributeValue {N = charge.TargetType.ToString()}},
                {"charge_group", new AttributeValue {S = charge.ChargeGroup.ToString()}},
                {"charge_year", new AttributeValue {N = charge.ChargeYear.ToString()}},
                {
                    "detailed_charges",
                    new AttributeValue
                    {
                        L=
                            charge.DetailedCharges.Select(p=>
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"chargeCode", new AttributeValue {S = p.ChargeCode}},
                                    {"frequency", new AttributeValue {S = p.Frequency}},
                                    {"amount", new AttributeValue {N = p.Amount.ToString("F")}},
                                    {"endDate", new AttributeValue {S = p.EndDate.ToString("F")}},
                                    {"chargeType", new AttributeValue {S = p.ChargeType.ToString()}},
                                    {"subType", new AttributeValue {S = p.SubType.ToString()}},
                                    {"type", new AttributeValue {S = p.Type.ToString()}},
                                    {"startDate", new AttributeValue {S = p.StartDate.ToString("F")}},
                                }
                            }).ToList()
                    }
                }
            };
        }

    }
}
