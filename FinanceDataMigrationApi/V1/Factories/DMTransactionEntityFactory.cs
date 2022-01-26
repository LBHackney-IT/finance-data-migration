using System;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class DMTransactionEntityFactory
    {
        public static DMTransactionEntity ToDatabase(this DMTransactionEntityDomain dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new DMTransactionEntity
            {
                Id = dMTransactionEntity.Id,
                BalanceAmount = dMTransactionEntity.BalanceAmount ?? 0,
                BankAccountNumber = dMTransactionEntity.BankAccountNumber,
                ChargedAmount = dMTransactionEntity.ChargedAmount ?? 0,
                CreatedAt = dMTransactionEntity.CreatedAt,
                FinancialMonth = dMTransactionEntity.FinancialMonth,
                FinancialYear = dMTransactionEntity.FinancialYear,
                Fund = dMTransactionEntity.Fund,
                HousingBenefitAmount = dMTransactionEntity.HousingBenefitAmount ?? 0,
                IdDynamodb = dMTransactionEntity.IdDynamodb,
                IsLoaded = dMTransactionEntity.IsLoaded,
                IsIndexed = dMTransactionEntity.IsIndexed,
                IsSuspense = dMTransactionEntity.IsSuspense,
                IsTransformed = dMTransactionEntity.IsTransformed,
                PaidAmount = dMTransactionEntity.PaidAmount ?? 0,
                PaymentReference = dMTransactionEntity.PaymentReference,
                PeriodNo = dMTransactionEntity.PeriodNo,
                Person = dMTransactionEntity.Person,
                SuspenseResolutionInfo = dMTransactionEntity.SuspenseResolutionInfo,
                TargetId = dMTransactionEntity.TargetId,
                TargetType = dMTransactionEntity.TargetType,
                TransactionAmount = dMTransactionEntity.TransactionAmount,
                TransactionDate = dMTransactionEntity.TransactionDate,
                TransactionSource = dMTransactionEntity.TransactionSource,
                TransactionType = dMTransactionEntity.TransactionType

            };
        }

        public static DMTransactionEntityDomain ToDomain(this DMTransactionEntity dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new DMTransactionEntityDomain
            {
                Id = dMTransactionEntity.Id,
                BalanceAmount = dMTransactionEntity.BalanceAmount,
                BankAccountNumber = dMTransactionEntity.BankAccountNumber,
                ChargedAmount = dMTransactionEntity.ChargedAmount,
                CreatedAt = dMTransactionEntity.CreatedAt,
                FinancialMonth = (short) dMTransactionEntity.FinancialMonth,
                FinancialYear = (short) dMTransactionEntity.FinancialYear,
                Fund = dMTransactionEntity.Fund,
                HousingBenefitAmount = dMTransactionEntity.HousingBenefitAmount,
                IdDynamodb = dMTransactionEntity.IdDynamodb,
                IsLoaded = dMTransactionEntity.IsLoaded,
                IsIndexed = dMTransactionEntity.IsIndexed ?? false,
                IsSuspense = dMTransactionEntity.IsSuspense,
                IsTransformed = dMTransactionEntity.IsTransformed,
                PaidAmount = dMTransactionEntity.PaidAmount,
                PaymentReference = dMTransactionEntity.PaymentReference,
                PeriodNo = dMTransactionEntity.PeriodNo,
                Person = dMTransactionEntity.Person,
                SuspenseResolutionInfo = dMTransactionEntity.SuspenseResolutionInfo,
                TargetId = dMTransactionEntity.TargetId ?? Guid.Empty,
                TargetType = dMTransactionEntity.TargetType,
                TransactionAmount = dMTransactionEntity.TransactionAmount,
                TransactionDate = dMTransactionEntity.TransactionDate,
                TransactionSource = dMTransactionEntity.TransactionSource,
                TransactionType = dMTransactionEntity.TransactionType
            };
        }

        public static List<DMTransactionEntityDomain> ToDomain(this IList<DMTransactionEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }

        public static List<DMTransactionEntity> ToDatabase(this IList<DMTransactionEntityDomain> dMTransactionEntityDomainItems)
        {
            return dMTransactionEntityDomainItems.Select(p => p.ToDatabase()).ToList();
        }

    }
}
