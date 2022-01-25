using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.V1.Factories
{
   public static class ChargeFactory
    {
        public static Charge ToDomain(this ChargeDbEntity chargeEntity)
        {
            if (chargeEntity == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeEntity.Id,
                TargetId = chargeEntity.TargetId,
                TargetType = chargeEntity.TargetType,
                ChargeGroup = chargeEntity.ChargeGroup,
                DetailedCharges = chargeEntity.DetailedCharges
            };
        }

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

        public static Charge ToDomain(this AddChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }

        public static Charge ToDomain(this UpdateChargeRequest chargeRequest)
        {
            if (chargeRequest == null)
            {
                return null;
            }

            return new Charge
            {
                Id = chargeRequest.Id,
                TargetId = chargeRequest.TargetId,
                TargetType = chargeRequest.TargetType,
                ChargeGroup = chargeRequest.ChargeGroup,
                DetailedCharges = chargeRequest.DetailedCharges
            };
        }
        public static List<ChargeDbEntity> ToDatabaseList(this List<Charge> charges)
        {
            return charges.Select(item => item.ToDatabase()).ToList();
        }
        public static List<Charge> ToDomainList(this List<AddChargeRequest> charges)
        {
            return charges.Select(item => item.ToDomain()).ToList();
        }

    }
}
