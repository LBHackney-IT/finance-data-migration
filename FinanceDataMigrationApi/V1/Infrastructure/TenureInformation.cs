using Hackney.Shared.HousingSearch.Domain.Tenure;
using System;
using System.Collections.Generic;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public class TenureInformation
    {
        public Guid Id { get; set; }
        public List<HouseholdMember> HouseHoldMembers { get; set; }
        public Guid TenuredAssetId { get; set; }
    }
}
