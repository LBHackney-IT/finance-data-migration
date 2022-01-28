using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Hackney.Shared.HousingSearch.Domain.Asset;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class AssetFactory
    {
        public static XElement ToXElement(this List<Asset> assets)
        {
            var xEle = new XElement("Assets",
                assets.Select(a => new XElement("Asset",
                    new XElement("id", a.Id),
                    new XElement("assetId", a.AssetId),
                    new XElement("assetType", a.AssetType),
                    new XElement("tenure_id", a.Tenure?.Id),
                    new XElement("tenure_paymentReference", a.Tenure?.PaymentReference)
                )));

            return xEle;
        }
    }
}
