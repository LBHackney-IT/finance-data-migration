using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.HousingSearch.Domain.Asset;
using Hackney.Shared.Tenure.Domain;
using Asset = Hackney.Shared.HousingSearch.Domain.Asset.Asset;
using AssetAddress = Hackney.Shared.Asset.Domain.AssetAddress;
using TenuredAsset = Hackney.Shared.Tenure.Domain.TenuredAsset;

namespace FinanceDataMigrationApi.V1.Infrastructure
{
    public static class ScanResponseExtension
    {
        public static IEnumerable<TenureInformation> ToTenureInformation(this ScanResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                yield return new TenureInformation
                {
                    Id = Guid.Parse(item["id"].S),

                    TenuredAsset = item.ContainsKey("tenuredAsset") ? new TenuredAsset()
                    {
                        FullAddress = item["tenuredAsset"].M.ContainsKey("fullAddress") ? (item["tenuredAsset"].M["fullAddress"].NULL ? null : item["tenuredAsset"].M["fullAddress"].S) : null
                    } : null,

                    TenureType = item.ContainsKey("tenureType") ? new TenureType()
                    {
                        Code = item["tenureType"].M.ContainsKey("code") ? (item["tenureType"].M["code"].NULL ? null : item["tenureType"].M["code"].S) : null,
                        Description = item["tenureType"].M.ContainsKey("description") ? (item["tenureType"].M["description"].NULL ? null : item["tenureType"].M["description"].S) : null
                    } : null,

                    Terminated = item.ContainsKey("terminated") ? new Terminated()
                    {
                        IsTerminated = item["terminated"].M.ContainsKey("isTerminated") && (item["terminated"].M["isTerminated"].NULL ? false : item["terminated"].M["isTerminated"].BOOL),
                        ReasonForTermination = item["terminated"].M.ContainsKey("reasonForTermination") ? (item["terminated"].M["reasonForTermination"].NULL ? null : item["terminated"].M["reasonForTermination"].S.Trim()) : null
                    } : null,

                    PaymentReference = item.ContainsKey("paymentReference") ? (item["paymentReference"].NULL ? null : item["paymentReference"].S) : null,
                    HouseholdMembers = item.ContainsKey("householdMembers") ?
                        item["householdMembers"].NULL ? null :
                        item["householdMembers"].L.Select(m =>
                           new HouseholdMembers
                           {
                               Id = m.M["id"].NULL ? Guid.Empty : Guid.Parse(m.M["id"].S),
                               FullName = m.M.ContainsKey("fullName") ? (m.M["fullName"].NULL ? null : m.M["fullName"].S) : null,
                               IsResponsible = m.M.ContainsKey("isResponsible") && (!m.M["isResponsible"].NULL && m.M["isResponsible"].BOOL)
                           }) : null
                };
            }
        }

        public static IEnumerable<Asset> ToAssets(this ScanResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                yield return new Asset
                {
                    Id = item["id"].S,
                    AssetId = item.ContainsKey("assetId") ? (item["assetId"].NULL ? null : item["assetId"].S) : null,
                    AssetType = item.ContainsKey("assetType") ? (item["assetType"].NULL ? null : item["assetType"].S) : null,
                    Tenure = item.ContainsKey("tenure") ? new Tenure()
                    {
                        Id = item["tenure"].M.ContainsKey("id") ? (item["tenure"].M["id"].NULL ? null : item["tenure"].M["id"].S) : null,
                        PaymentReference = item["tenure"].M.ContainsKey("paymentReference") ? (item["tenure"].M["paymentReference"].NULL ? null : item["tenure"].M["paymentReference"].S) : null,
                    } : null
                };
            }
        }

        public static IEnumerable<Hackney.Shared.Asset.Domain.Asset> ToAssetsDomain(this ScanResponse response)
        {
            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                var assetLocation = new List<ParentAsset>();
                foreach (var childItem in item["assetLocation"].L)
                {
                    var data = new ParentAsset()
                    {
                        Type = childItem.M["type"].S,
                        Id = Guid.Parse(childItem.M["id"].S),
                        Name = childItem.M["name"].S,
                    };
                    assetLocation.Add(data);
                }

                yield return new Hackney.Shared.Asset.Domain.Asset
                {
                    Id = Guid.Parse(item["id"].S),
                    AssetId = item.ContainsKey("assetId") ? (item["assetId"].NULL ? null : item["assetId"].S) : null,
                    AssetType = Enum.Parse<AssetType>(item["assetType"].S),
                    Tenure = item.ContainsKey("tenure") ? new AssetTenure()
                    {
                        Id = item["tenure"].M.ContainsKey("id") ? (item["tenure"].M["id"].NULL ? null : item["tenure"].M["id"].S) : null,
                        PaymentReference = item["tenure"].M.ContainsKey("paymentReference") ? (item["tenure"].M["paymentReference"].NULL ? null : item["tenure"].M["paymentReference"].S) : null,
                        Type = item["tenure"].M.ContainsKey("type") ? (item["tenure"].M["type"].NULL ? null : item["tenure"].M["type"].S) : null,
                        EndOfTenureDate = item["tenure"].M.ContainsKey("endOfTenureDate")
                            ? Convert.ToDateTime(item["tenure"].M["endOfTenureDate"].S)
                            : (DateTime?) null,
                        StartOfTenureDate = item["tenure"].M.ContainsKey("startOfTenureDate")
                            ? Convert.ToDateTime(item["tenure"].M["startOfTenureDate"].S)
                            : (DateTime?) null

                    } : null,
                    AssetAddress = item.ContainsKey("assetAddress") ? new AssetAddress()
                    {
                        AddressLine1 = item["assetAddress"].M.ContainsKey("addressLine1") ? (item["assetAddress"].M["addressLine1"].NULL ? null : item["assetAddress"].M["addressLine1"].S) : null,
                        AddressLine2 = item["assetAddress"].M.ContainsKey("addressLine2") ? (item["assetAddress"].M["addressLine2"].NULL ? null : item["assetAddress"].M["addressLine2"].S) : null,
                        AddressLine3 = item["assetAddress"].M.ContainsKey("addressLine3") ? (item["assetAddress"].M["addressLine3"].NULL ? null : item["assetAddress"].M["addressLine3"].S) : null,
                        AddressLine4 = item["assetAddress"].M.ContainsKey("addressLine4") ? (item["assetAddress"].M["addressLine4"].NULL ? null : item["assetAddress"].M["addressLine4"].S) : null,
                        PostCode = item["assetAddress"].M.ContainsKey("postCode") ? (item["assetAddress"].M["postCode"].NULL ? null : item["assetAddress"].M["postCode"].S) : null,
                        PostPreamble = item["assetAddress"].M.ContainsKey("postPreamble") ? (item["assetAddress"].M["postPreamble"].NULL ? null : item["assetAddress"].M["postPreamble"].S) : null,
                        Uprn = item["assetAddress"].M.ContainsKey("uprn") ? (item["assetAddress"].M["uprn"].NULL ? null : item["assetAddress"].M["uprn"].S) : null
                    } : null,
                    AssetCharacteristics = item.ContainsKey("assetCharacteristics") ? new AssetCharacteristics()
                    {
                        NumberOfBedrooms = item["assetCharacteristics"].M.ContainsKey("numberOfBedrooms") ? Convert.ToInt32(item["assetCharacteristics"].M["numberOfBedrooms"].N) : 0,
                        NumberOfLifts = item["assetCharacteristics"].M.ContainsKey("numberOfLifts") ? Convert.ToInt32(item["assetCharacteristics"].M["numberOfLifts"].N) : 0,
                        NumberOfLivingRooms = item["assetCharacteristics"].M.ContainsKey("numberOfLivingRooms") ? Convert.ToInt32(item["assetCharacteristics"].M["numberOfLivingRooms"].N) : 0,
                        WindowType = item["assetCharacteristics"].M.ContainsKey("windowType") ? item["assetCharacteristics"].M["windowType"].S : null,
                        YearConstructed = item["assetCharacteristics"].M.ContainsKey("yearConstructed") ? item["assetCharacteristics"].M["yearConstructed"].S : null,

                    } : null,
                    AssetLocation = item.ContainsKey("assetLocation") ? new AssetLocation()
                    {
                        FloorNo = item["assetLocation"].M.ContainsKey("floorNo") ? item["assetLocation"].M["floorNo"].S : null,
                        TotalBlockFloors = item["assetLocation"].M.ContainsKey("totalBlockFloors") ? Convert.ToInt32(item["assetLocation"].M["totalBlockFloors"].N) : 0,
                        ParentAssets = item["assetLocation"].L.Any() ? assetLocation : null,

                    } : null,
                    ParentAssetIds = item.ContainsKey("parentAssetIds") ? (item["parentAssetIds"].NULL ? null : item["parentAssetIds"].S) : null,
                    RootAsset = item.ContainsKey("rootAsset") ? (item["rootAsset"].NULL ? null : item["rootAsset"].S) : null,
                };
            }
        }
    }
}
