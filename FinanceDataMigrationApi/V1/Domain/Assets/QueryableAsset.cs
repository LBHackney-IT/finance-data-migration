using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.HousingSearch.Domain.Asset;
using Hackney.Shared.HousingSearch.Gateways.Models.Assets;
using Nest;

namespace FinanceDataMigrationApi.V1.Domain.Assets
{
    public class QueryableAsset
    {

        [Text(Name = "id")]
        public string Id { get; set; }

        [Text(Name = "assetId")]
        public string AssetId { get; set; }

        [Text(Name = "assetType")]
        public string AssetType { get; set; }

        [Text(Name = "isAssetCautionaryAlerted")]
        public bool IsAssetCautionaryAlerted { get; set; }

        [Text(Name = "rootAsset")]
        public string RootAsset { get; set; }

        [Text(Name = "parentAssetIds")]
        public string ParentAssetIds { get; set; }

        public QueryableAssetAddress AssetAddress { get; set; }

        public QueryableAssetTenure Tenure { get; set; }

        public QueryableAssetLocation AssetLocation { get; set; }

        public QueryableAssetCharacteristics AssetCharacteristics { get; set; }

        //public QueryableAssetManagement AssetManagement { get; set; }
    }
}
