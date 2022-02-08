using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain.Accounts;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class AccountsFactory
    {
        public static Dictionary<string, AttributeValue> ToQueryRequest(this DmAccount account)
        {
            var accountModel = new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = account.DynamoDbId.ToString()}},
                {"account_balance", new AttributeValue {N = account.AccountBalance.HasValue ? account.AccountBalance.Value.ToString("F").Replace(',', '.') : "0"}},
                {"target_id", new AttributeValue {S = account.TargetId.ToString()}},
                {"target_type", new AttributeValue {S = account.TargetType.ToString()}},
                {"account_type", new AttributeValue {S = account.AccountType?.ToString()??""}},
                {"rent_group_type", new AttributeValue {S = account.RentGroupType?.ToString()??""}},
                {"agreement_type", new AttributeValue {S = account.AgreementType?.ToString()??""}},
                {"created_at", new AttributeValue {S = account.CreatedAt.ToString("F")}},
                /*{"created_by", new AttributeValue {S = account.CreatedBy}},
                {"last_updated_by", new AttributeValue {S = account.LastUpdatedBy}},
                {"last_updated_at", new AttributeValue {S = account.LastUpdatedAt.ToString()}},*/
                {"start_date", new AttributeValue {S = account.StartDate.ToString("F")}},
                {"end_date", new AttributeValue {S = account.EndDate?.ToString()??""}},
                {"account_status", new AttributeValue {S = account.AccountStatus?.ToString()??""}},
                {"payment_reference", new AttributeValue {S = account.PaymentReference??""}},
                {"end_reason_code", new AttributeValue{S = account.EndReasonCode?.ToString()??""}},
                {"parent_account_id", new AttributeValue {S = account.ParentAccountId?.ToString()??""}},
                {
                    "tenure", account.Tenure==null?new AttributeValue(""):
                        new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                {"tenureId",new AttributeValue{S = account.Tenure?.Id.ToString()??""}},
                                {
                                    "tenureType", account.Tenure.TenureTypeCode==null?new AttributeValue("") :
                                    new AttributeValue
                                    {
                                        M=new Dictionary<string, AttributeValue>
                                        {
                                            {"code",new AttributeValue(account.Tenure.TenureTypeCode??"")},
                                            {"description",new AttributeValue(account.Tenure.TenureTypeDesc??"")}
                                        }
                                    }
                                },
                                {"fullAddress",new AttributeValue(account.Tenure.FullAddress)},
                                {
                                    "primaryTenants",account.Tenure.PrimaryTenants==null?new AttributeValue("") :
                                        new AttributeValue
                                        {
                                            L=account.Tenure.PrimaryTenants.Select(p=>
                                                new AttributeValue
                                                {
                                                    M = new Dictionary<string, AttributeValue>
                                                    {
                                                        {"id",new AttributeValue(p.Id.ToString())},
                                                        {"fullName",new AttributeValue(p.FullName?.ToString()??"")}
                                                    }
                                                }
                                            ).ToList()
                                        }
                                }
                            }
                        }
                }/*,
                {
                    "consolidatedCharges",account.ConsolidatedCharges==null?new AttributeValue
                        {
                            L=new List<AttributeValue>
                            {
                                new AttributeValue()
                                {
                                    M = new Dictionary<string, AttributeValue>
                                    {
                                        {}
                                    }
                                }
                            }
                        } : 
                        new AttributeValue
                        {
                            L=account.ConsolidatedCharges.Select(p=>
                                new AttributeValue
                                {
                                    M=new Dictionary<string, AttributeValue>
                                    {
                                        {"type",new AttributeValue(p.Type?.ToString()??"")},
                                        {"frequency",new AttributeValue(p.Frequency?.ToString()??"")},
                                        {"amount",new AttributeValue{N=p.Amount.ToString("F")}}
                                    }
                                }).ToList()
                        }
                }*/
            };

            if (account.ConsolidatedCharges != null && account.ConsolidatedCharges.Count > 0)
            {
                var consolidatedCharges =
                    new AttributeValue
                    {
                        L = account.ConsolidatedCharges.Select(p =>
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    {"type", new AttributeValue(p.Type?.ToString() ?? "")},
                                    {"frequency", new AttributeValue(p.Frequency?.ToString() ?? "")},
                                    {"amount", new AttributeValue {N = p.Amount.ToString("F")}}
                                }
                            }).ToList()
                    };

                accountModel.Add("consolidatedCharges", consolidatedCharges);
            }

            return accountModel;
        }

        public static DmAccount ToDomain(this DmAccountDbEntity accountDbEntity)
        {
            return new DmAccount
            {
                ConsolidatedCharges = accountDbEntity.ConsolidatedCharges.Select(c =>
                    new DmConsolidatedCharge
                    {
                        AccountId = c.AccountId,
                        PaymentReference = c.PaymentReference,
                        Amount = c.Amount,
                        Frequency = c.Frequency,
                        Id = c.Id,
                        Type = c.Type
                    }).ToList(),
                PaymentReference = accountDbEntity.PaymentReference,
                ConsolidatedBalance = accountDbEntity.ConsolidatedBalance,
                Id = accountDbEntity.Id,
                AccountBalance = accountDbEntity.AccountBalance,
                AccountStatus = accountDbEntity.AccountStatus,
                AccountType = accountDbEntity.AccountType,
                AgreementType = accountDbEntity.AgreementType,
                CreatedAt = accountDbEntity.CreatedAt,
                DynamoDbId = accountDbEntity.DynamoDbId,
                EndDate = accountDbEntity.EndDate,
                EndReasonCode = accountDbEntity.EndReasonCode,
                IsIndexed = accountDbEntity.IsIndexed,
                MigrationStatus = accountDbEntity.MigrationStatus,
                ParentAccountId = accountDbEntity.ParentAccountId,
                RentGroupType = accountDbEntity.RentGroupType,
                StartDate = accountDbEntity.StartDate,
                TargetId = accountDbEntity.TargetId,
                TargetType = accountDbEntity.TargetType,
                Tenure = accountDbEntity.Tenure?.ToDomain()
            };
        }

        public static List<DmAccount> ToDomain(this IList<DmAccountDbEntity> accountDbEntities)
        {
            return accountDbEntities.Select(p => p.ToDomain()).ToList();
        }
    }
}
