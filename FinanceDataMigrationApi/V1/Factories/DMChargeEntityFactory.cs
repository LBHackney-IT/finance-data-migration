using System;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class DMChargeEntityFactory
    {
        public static DMChargeEntity ToDatabase(this DMChargeEntityDomain dMChargeEntityDomain)
        {
            return dMChargeEntityDomain == null
                ? null
                : new DMChargeEntity()
                {
                    Id = dMChargeEntityDomain.Id,
                    IdDynamodb = dMChargeEntityDomain.IdDynamodb,
                    TargetId = dMChargeEntityDomain.TargetId,
                    PaymentReference = dMChargeEntityDomain.PaymentReference,
                    TargetType = dMChargeEntityDomain.TargetType,
                    ChargeGroup = dMChargeEntityDomain.ChargeGroup,
                    DetailedCharges = dMChargeEntityDomain.DetailedCharges,
                    Type = dMChargeEntityDomain.Type,
                    SubType = dMChargeEntityDomain.SubType,
                    ChargeType = dMChargeEntityDomain.ChargeType,
                    Frequency = dMChargeEntityDomain.Frequency,
                    Amount = dMChargeEntityDomain.Amount ?? 0,
                    ChargeCode = dMChargeEntityDomain.ChargeCode,
                    StartDate = dMChargeEntityDomain.StartDate,
                    EndDate = dMChargeEntityDomain.EndDate,
                    IsTransformed = dMChargeEntityDomain.IsTransformed,
                    IsLoaded = dMChargeEntityDomain.IsLoaded,
                    CreatedAt = dMChargeEntityDomain.CreatedAt
                };
        }

        public static DMChargeEntityDomain ToDomain(this DMChargeEntity dMChargeEntity)
        {
            return dMChargeEntity == null
                ? null
                : new DMChargeEntityDomain()
                {
                    Id = dMChargeEntity.Id,
                    IdDynamodb = dMChargeEntity.IdDynamodb,
                    TargetId = dMChargeEntity.TargetId ?? Guid.Empty,
                    PaymentReference = dMChargeEntity.PaymentReference,
                    TargetType = dMChargeEntity.TargetType,
                    ChargeGroup = dMChargeEntity.ChargeGroup,
                    DetailedCharges = dMChargeEntity.DetailedCharges,
                    Type = dMChargeEntity.Type,
                    SubType = dMChargeEntity.SubType,
                    ChargeType = dMChargeEntity.ChargeType,
                    Frequency = dMChargeEntity.Frequency,
                    Amount = dMChargeEntity.Amount ?? 0,
                    ChargeCode = dMChargeEntity.ChargeCode,
                    StartDate = dMChargeEntity.StartDate,
                    EndDate = dMChargeEntity.EndDate,
                    IsTransformed = dMChargeEntity.IsTransformed,
                    IsLoaded = dMChargeEntity.IsLoaded,
                    CreatedAt = dMChargeEntity.CreatedAt
                };
        }

        public static List<DMChargeEntityDomain> ToDomain(this IList<DMChargeEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }

        public static List<DMChargeEntity> ToDatabase(this IList<DMChargeEntityDomain> dMChargeEntityDomainItems)
        {
            return dMChargeEntityDomainItems.Select(p => p.ToDatabase()).ToList();
        }
    }
}
