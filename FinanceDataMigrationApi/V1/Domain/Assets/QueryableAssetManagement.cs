using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Domain.Assets
{
    public class QueryableAssetManagement
    {
        public string Agent { get; set; }
        public string AreaOfficeName { get; set; }
        public bool IsCouncilProperty { get; set; }
        public string ManagingOrganisation { get; set; }
        public Guid ManagingOrganisationId { get; set; }
        public string Owner { get; set; }
        public bool IsTMOManaged { get; set; }
        public string PropertyOccupiedStatus { get; set; }
        public bool IsNoRepairsMaintenance { get; set; }
    }
}
