using System;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FinanceDataMigrationApi.V1.Infrastructure.Extentions;
using Hackney.Shared.HousingSearch.Domain.Accounts.Enum;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Accounts;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using Newtonsoft.Json;
using TargetType = Hackney.Shared.HousingSearch.Domain.Accounts.Enum.TargetType;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class EsFactory
    {
        public static QueryableTransaction ToQueryableTransaction(this Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            //if (transaction. == null)
            //    throw new Exception("There is no tenure provided for this asset.");

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
                //IsSuspense = transaction.IsSuspense,
                PaidAmount = transaction.PaidAmount,
                PaymentReference = transaction.PaymentReference,
                PeriodNo = transaction.PeriodNo,
                Sender = transaction.Sender != null ? new QueryableSender
                {
                    FullName = transaction.Sender.FullName,
                    Id = transaction.Sender.Id
                }: null,
                SuspenseResolutionInfo = transaction.SuspenseResolutionInfo != null? new QueryableSuspenseResolutionInfo
                {
                    IsApproved = transaction.SuspenseResolutionInfo.IsApproved,
                    IsConfirmed = transaction.SuspenseResolutionInfo.IsConfirmed,
                    Note = transaction.SuspenseResolutionInfo.Note,
                    ResolutionDate = transaction.SuspenseResolutionInfo.ResolutionDate
                }: null,
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

        public static QueryableAccount ToQueryableAccount(this DMAccountEntity accountEntity)
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
                CreatedBy = "Migration",
                CreatedAt = DateTime.UtcNow,
                //LastUpdatedBy = accountEntity.LastUpdatedBy,
                //LastUpdatedAt = accountEntity.LastUpdatedAt,
                StartDate = accountEntity.StartDate,
                TargetId = accountEntity.TargetId.ValueOrDefault(),
                TargetType = accountEntity.TargetType.ToEnumValue<TargetType>(),
                AccountType = accountEntity.AccountType.ToEnumValue<AccountType>(),
                AgreementType = accountEntity.AgreementType,
                RentGroupType = RentGroupType.Garages//accountEntity.RentGroupType.ToEnumValue<RentGroupType>(),
                // ToDo: define models
                //ConsolidatedCharges = JsonConvert.DeserializeObject<>(accountEntity.ConsolidatedCharges),
                //Tenure = JsonConvert.DeserializeObject<>(accountEntity.Tenure)
            };
        }

        public static Guid ValueOrDefault(this Guid? value)
            => value.HasValue ? value.Value : Guid.Empty;

        public static decimal ValueOrDefault(this decimal? value)
            => value.HasValue ? value.Value : 0;

        public static List<QueryableAccount> ToAccountRequestList(IEnumerable<DMAccountEntity> accounts)
        {
            var transactionRequestList = accounts.Select(item => item.ToQueryableAccount()).ToList();
            return transactionRequestList;
        }
    }
}
