using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.Asset.Domain;

namespace FinanceDataMigrationApi.V1.Domain.Assets
{
    public class QueryableAssetLocation
    {
        public string FloorNo { get; set; }
        public int TotalBlockFloors { get; set; }
        public IEnumerable<ParentAsset> ParentAssets { get; set; }
    }
}
