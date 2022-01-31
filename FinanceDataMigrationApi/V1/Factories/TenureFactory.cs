using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Hackney.Shared.Tenure.Domain;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Handlers;
using System;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class TenureFactory
    {

        public static XElement ToXElement(this List<TenureInformation> tenures)
        {
            try
            {
                LoggingHandler.LogInfo($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ToXElement)}: Converting list to XML.");
                var xEle = new XElement("Tenures",
                    tenures.Select(a => new XElement("Tenure",
                        new XElement("id", a.Id),
                        new XElement("payment_reference", a.PaymentReference?.Replace("'", "''")),
                        new XElement("tenure_type_code", a.TenureType?.Code?.Replace("'", "''")),
                        new XElement("tenure_type_desc", a.TenureType?.Description?.Replace("'", "''")),
                        new XElement("tenured_asset_full_address", a.TenuredAsset?.FullAddress?.Replace("'", "''")),
                        new XElement("terminated_reason_code", a.Terminated?.ReasonForTermination?.Replace("'", "''")),
                        a.HouseholdMembers?.Select(h =>
                            new XElement("HouseHoldMembers",
                                new XElement("id", h.Id),
                                new XElement("fullname", h.FullName?.Replace("'", "''")),
                                new XElement("is_responsible", h.IsResponsible))
                        )
                    )));

                return xEle;
            }
            catch (Exception ex)
            {
                LoggingHandler.LogError($"{nameof(FinanceDataMigrationApi)}.{nameof(Handler)}.{nameof(ToXElement)}: xml Converting error: {ex.Message}.");
                throw;
            }
        }

        public static Dictionary<string, AttributeValue> ToQueryRequest(this TenureInformation tenure)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = tenure.Id.ToString()}},
                {
                    "householdMembers",
                    new AttributeValue
                    {
                        L = tenure.HouseholdMembers.Select(p =>new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                {"id",new AttributeValue{S = p.Id.ToString()}},
                                {"fullName",new AttributeValue{S = p.FullName}},
                                {"isResponsible", new AttributeValue{BOOL = p.IsResponsible}},
                                {"dateOfBirth",new AttributeValue{S = p.DateOfBirth.ToString("F")}},
                                {"personTenureType",new AttributeValue{S = p.PersonTenureType.ToString()}},
                                {"type",new AttributeValue{S = p.Type.ToString()}}
                            }
                        }).ToList()
                    }
                },
                {"informHousingBenefitsForChanges",new AttributeValue{BOOL = tenure.InformHousingBenefitsForChanges??false}},
                {"tenureType",
                    new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            {"code",new AttributeValue{S = tenure.TenureType.Code}},
                            {"description",new AttributeValue{S = tenure.TenureType.Description}}
                        }
                    }
                },
                {"tenuredAsset",
                    new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            {"id",new AttributeValue{S = tenure.TenuredAsset.Id.ToString()}},
                            {"fullAddress",new AttributeValue{S = tenure.TenuredAsset.FullAddress}},
                            {"uprn",new AttributeValue{S = tenure.TenuredAsset.Uprn}},
                            {"propertyReference",new AttributeValue{S = tenure.TenuredAsset.PropertyReference}},
                            {"type",new AttributeValue{S = tenure.TenuredAsset.Type.ToString()}}
                        }
                    }
                },

                {"paymentReference", new AttributeValue {S = tenure.PaymentReference.ToString()}}
            };
        }
    }
}
