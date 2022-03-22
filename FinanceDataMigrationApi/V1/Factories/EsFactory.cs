using System;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Domain.Assets;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Assets;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Newtonsoft.Json;
using QueryableAsset = FinanceDataMigrationApi.V1.Domain.Assets.QueryableAsset;
using TargetType = Hackney.Shared.HousingSearch.Domain.Accounts.Enum.TargetType;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class EsFactory
    {
        public static QueryableTransaction ToQueryableTransaction(this Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            return new QueryableTransaction
            {
                Id = transaction.Id,
                Address = transaction.Address,
                BalanceAmount = transaction.BalanceAmount,
                BankAccountNumber = transaction.BankAccountNumber,
                ChargedAmount = transaction.ChargedAmount,
                FinancialMonth = transaction.FinancialMonth,
                Fund = transaction.Fund,
                HousingBenefitAmount = transaction.HousingBenefitAmount,
                /*IsSuspense = transaction.IsSuspense,*/
                PaidAmount = transaction.PaidAmount,
                PaymentReference = transaction.PaymentReference,
                PeriodNo = transaction.PeriodNo,
                Sender = transaction.Sender != null ? new QueryableSender
                {
                    FullName = transaction.Sender.FullName,
                    Id = transaction.Sender.Id
                } : null,
                SuspenseResolutionInfo = transaction.SuspenseResolutionInfo != null ? new QueryableSuspenseResolutionInfo
                {
                    IsApproved = transaction.SuspenseResolutionInfo.IsApproved,
                    IsConfirmed = transaction.SuspenseResolutionInfo.IsConfirmed,
                    Note = transaction.SuspenseResolutionInfo.Note,
                    ResolutionDate = transaction.SuspenseResolutionInfo.ResolutionDate
                } : null,
                TargetId = transaction.TargetId,
                TargetType = transaction.TargetType,
                TransactionAmount = transaction.TransactionAmount,
                TransactionDate = transaction.TransactionDate,
                TransactionSource = transaction.TransactionSource,
                TransactionType = transaction.TransactionType
            };
        }

        public static List<QueryableTransaction> ToTransactionRequestList(IEnumerable<Transaction> transactions)
        {
            var transactionRequestList = transactions.Select(item => item.ToQueryableTransaction()).ToList();
            return transactionRequestList;
        }

        public static QueryableAsset ToQueryableAsset(this Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            return new QueryableAsset
            {
                Id = asset.Id.ToString(),
                AssetId = asset.AssetId,
                AssetAddress = asset?.AssetAddress != null ? new QueryableAssetAddress()
                {
                    AddressLine1 = asset?.AssetAddress?.AddressLine1,
                    AddressLine2 = asset?.AssetAddress?.AddressLine2,
                    AddressLine3 = asset?.AssetAddress?.AddressLine3,
                    AddressLine4 = asset?.AssetAddress?.AddressLine4,
                    PostCode = asset?.AssetAddress?.PostCode,
                    PostPreamble = asset?.AssetAddress?.PostPreamble,
                    Uprn = asset?.AssetAddress?.Uprn
                } : null,
                Tenure = asset.Tenure != null ? new QueryableAssetTenure()
                {
                    Id = asset?.Tenure?.Id,
                    Type = asset?.Tenure?.Type,
                    StartOfTenureDate = asset?.Tenure?.StartOfTenureDate?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    EndOfTenureDate = asset?.Tenure?.EndOfTenureDate?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PaymentReference = asset?.Tenure?.PaymentReference
                } : null,
                AssetCharacteristics = asset?.AssetCharacteristics != null ? new QueryableAssetCharacteristics()
                {
                    NumberOfLifts = asset?.AssetCharacteristics?.NumberOfLifts ?? 0,
                    NumberOfLivingRooms = asset?.AssetCharacteristics?.NumberOfLivingRooms ?? 0,
                    WindowType = asset?.AssetCharacteristics?.WindowType,
                    YearConstructed = asset?.AssetCharacteristics?.YearConstructed,
                    NumberOfBedrooms = asset?.AssetCharacteristics?.NumberOfBedrooms ?? 0
                } : null,
                ParentAssetIds = asset?.ParentAssetIds,
                RootAsset = asset?.RootAsset,
                AssetType = asset?.AssetType.ToString(),
                IsAssetCautionaryAlerted = false,
                AssetManagement = asset?.AssetManagement != null ? new QueryableAssetManagement()
                {
                    AreaOfficeName = asset?.AssetManagement?.AreaOfficeName,
                    IsCouncilProperty = asset?.AssetManagement?.IsCouncilProperty ?? false,
                    ManagingOrganisation = asset?.AssetManagement?.ManagingOrganisation,
                    ManagingOrganisationId = asset?.AssetManagement?.ManagingOrganisationId ?? Guid.Empty,
                    Owner = asset?.AssetManagement?.Owner,
                    IsTMOManaged = asset?.AssetManagement?.IsTMOManaged ?? false,
                    PropertyOccupiedStatus = asset?.AssetManagement?.PropertyOccupiedStatus,
                    IsNoRepairsMaintenance = asset?.AssetManagement?.IsNoRepairsMaintenance ?? false,
                    Agent = asset?.AssetManagement?.Agent
                } : null
            };
        }

        public static List<QueryableAsset> ToAssetRequestList(IEnumerable<Asset> assets)
        {
            var transactionRequestList = assets.Select(item => item.ToQueryableAsset()).ToList();
            return transactionRequestList;
        }

        public static QueryableAccount ToQueryableAccount(this DmAccountDbEntity accountEntity)
        {
            return new QueryableAccount
            {
                Id = accountEntity.DynamoDbId,
                ParentAccountId = accountEntity.ParentAccountId.ValueOrDefault(),
                PaymentReference = accountEntity.PaymentReference,
                //EndReasonCode = accountEntity.EndReasonCode, // ToDo: find EndReasonCode in Shared package
                AccountBalance = accountEntity.AccountBalance.ValueOrDefault(),
                ConsolidatedBalance = accountEntity.ConsolidatedBalance.ValueOrDefault(),
                AccountStatus = accountEntity.AccountStatus.ToEnumValue<AccountStatus>(),
                EndDate = accountEntity.EndDate,
                CreatedBy = "Migration", // ToDo: what should we specify here?
                CreatedAt = DateTime.UtcNow,
                //LastUpdatedBy = accountEntity.LastUpdatedBy,
                //LastUpdatedAt = accountEntity.LastUpdatedAt,
                StartDate = accountEntity.StartDate,
                TargetId = accountEntity.TargetId.ValueOrDefault(),
                TargetType = accountEntity.TargetType.ToEnumValue<TargetType>(),
                AccountType = accountEntity.AccountType.ToEnumValue<AccountType>(),
                AgreementType = accountEntity.AgreementType,
                RentGroupType = accountEntity.RentGroupType.ToRentGroup() /*RentGroupType.Garages,*/ /*accountEntity.RentGroupType.ToEnumValue<RentGroupType>()*/,
                /*ConsolidatedCharges = accountEntity.ConsolidatedBalance,
                Tenure = accountEntity.Tenure*/
            };
        }

        private static T DeserializeOrDefault<T>(string json) where T : class
            => json == null ? null : JsonConvert.DeserializeObject<T>(json);

        private static Guid ValueOrDefault(this Guid? value)
            => value.HasValue ? value.Value : Guid.Empty;

        private static decimal ValueOrDefault(this decimal? value)
            => value.HasValue ? value.Value : 0;

        public static List<QueryableAccount> ToAccountRequestList(IEnumerable<DmAccountDbEntity> accounts)
        {
            var transactionRequestList = accounts.Select(item => item.ToQueryableAccount()).ToList();
            return transactionRequestList;
        }
    }
}
