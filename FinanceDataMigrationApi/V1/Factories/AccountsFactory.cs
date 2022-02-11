using System;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain.Accounts;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class AccountsFactory
    {
        public static Dictionary<string, AttributeValue> ToQueryRequest(this DmAccount account)
        {
            var accountModel = new Dictionary<string, AttributeValue>();

            accountModel.PureAdd("id", new AttributeValue { S = account.DynamoDbId.ToString() });
            accountModel.PureAdd("target_id", new AttributeValue { S = account.TargetId?.ToString() });
            accountModel.PureAdd("account_balance", new AttributeValue { N = account.AccountBalance?.ToString("F").Replace(',', '.') });
            accountModel.PureAdd("target_type", new AttributeValue { S = account.TargetType.ToString() });
            accountModel.PureAdd("account_type", new AttributeValue { S = account.AccountType?.ToString() });
            accountModel.PureAdd("rent_group_type", new AttributeValue { S = account.RentGroupType?.ToString() });
            accountModel.PureAdd("agreement_type", new AttributeValue { S = account.AgreementType?.ToString() });
            accountModel.PureAdd("created_at", new AttributeValue { S = account.CreatedAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            accountModel.PureAdd("start_date", new AttributeValue { S = account.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            accountModel.PureAdd("end_date", new AttributeValue { S = account.EndDate?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            accountModel.PureAdd("account_status", new AttributeValue { S = account.AccountStatus?.ToString() });
            accountModel.PureAdd("payment_reference", new AttributeValue { S = account.PaymentReference });
            accountModel.PureAdd("end_reason_code", new AttributeValue { S = account.EndReasonCode?.ToString() });
            accountModel.PureAdd("parent_account_id", new AttributeValue { S = account.ParentAccountId?.ToString() });
            if (account.Tenure != null)
            {
                var accountTenureModel = new Dictionary<string, AttributeValue>();
                accountTenureModel.PureAdd("tenureId", new AttributeValue { S = account.Tenure?.Id.ToString() });
                accountTenureModel.PureAdd("fullAddress",
                    new AttributeValue { S = account.Tenure?.FullAddress.ToString() });

                var accountTenureTenureTypeModel = new Dictionary<string, AttributeValue>();
                accountTenureTenureTypeModel.PureAdd("code", new AttributeValue(account.Tenure?.TenureTypeCode));
                accountTenureTenureTypeModel.PureAdd("description", new AttributeValue(account.Tenure?.TenureTypeDesc));
                accountTenureModel.PureAdd("tenureType", new AttributeValue { M = accountTenureTenureTypeModel });

                if (account.Tenure?.PrimaryTenants != null && account.Tenure?.PrimaryTenants.Count > 0)
                {
                    var accountTenurePrimaryTenantsModelList = new List<AttributeValue>();
                    foreach (var primaryTenant in account.Tenure.PrimaryTenants)
                    {
                        var accountTenurePrimaryTenantsModel = new Dictionary<string, AttributeValue>();
                        accountTenurePrimaryTenantsModel.PureAdd("id", new AttributeValue(primaryTenant.Id.ToString()));
                        accountTenurePrimaryTenantsModel.PureAdd("fullName", new AttributeValue(primaryTenant.FullName));
                        accountTenurePrimaryTenantsModelList.Add(new AttributeValue
                        {
                            M = accountTenurePrimaryTenantsModel
                        });
                    }

                    accountTenureModel.PureAdd("primaryTenants",
                        new AttributeValue
                        {
                            L = accountTenurePrimaryTenantsModelList
                        });
                }

                accountModel.PureAdd("tenure", new AttributeValue { M = accountTenureModel });
            }
            accountModel.PureAdd("last_updated_at", new AttributeValue { S = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            accountModel.PureAdd("parent_account_id", new AttributeValue { S = Guid.Empty.ToString() });

            /*{"created_by", new AttributeValue {S = account.CreatedBy}},
            {"last_updated_by", new AttributeValue {S = account.LastUpdatedBy}},
            {"last_updated_at", new AttributeValue {S = account.LastUpdatedAt.ToString()}},*/


            if (account.ConsolidatedCharges != null && account.ConsolidatedCharges.Count > 0)
            {
                var accountConsolidatedChargesModelList = new List<AttributeValue>();
                foreach (DmConsolidatedCharge cc in account.ConsolidatedCharges)
                {

                    var accountConsolidatedChargesModel = new Dictionary<string, AttributeValue>();


                    accountConsolidatedChargesModel.PureAdd("type", new AttributeValue(cc.Type?.ToString()));
                    accountConsolidatedChargesModel.PureAdd("frequency", new AttributeValue(cc.Frequency?.ToString()));
                    accountConsolidatedChargesModel.PureAdd("amount", new AttributeValue { N = cc.Amount.ToString("F") });

                    accountConsolidatedChargesModelList.Add(new AttributeValue
                    {
                        M = accountConsolidatedChargesModel
                    });
                }

                accountModel.Add("consolidatedCharges", new AttributeValue { L = accountConsolidatedChargesModelList });
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
