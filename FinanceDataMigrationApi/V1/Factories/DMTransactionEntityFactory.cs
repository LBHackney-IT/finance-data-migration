using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class DMTransactionEntityFactory
    {
        public static DMTransactionEntity ToDatabase(this DMTransactionEntityDomain dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new DMTransactionEntity
            {
                Id = dMTransactionEntity.Id,
                BalanceAmount  =  dMTransactionEntity.BalanceAmount,
                BankAccountNumber = dMTransactionEntity.BankAccountNumber,
                ChargedAmount = dMTransactionEntity.ChargedAmount,
                CreatedAt = dMTransactionEntity.CreatedAt,
                FinancialMonth = dMTransactionEntity.FinancialMonth,
                FinancialYear = dMTransactionEntity.FinancialYear,
                Fund = dMTransactionEntity.Fund,
                HousingBenefitAmount = dMTransactionEntity.HousingBenefitAmount,
                IdDynamodb = dMTransactionEntity.IdDynamodb,
                IsLoaded = dMTransactionEntity.IsLoaded,
                IsSuspense = dMTransactionEntity.IsSuspense,
                IsTransformed = dMTransactionEntity.IsTransformed,
                PaidAmount = dMTransactionEntity.PaidAmount,
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

        public static DMTransactionEntityDomain ToDomain(this DMTransactionEntity dMTransactionEntityDomain)
        {
            return dMTransactionEntityDomain == null ? null : new DMTransactionEntityDomain
            {
                Id = dMTransactionEntityDomain.Id,
                BalanceAmount = dMTransactionEntityDomain.BalanceAmount,
                BankAccountNumber = dMTransactionEntityDomain.BankAccountNumber,
                ChargedAmount = dMTransactionEntityDomain.ChargedAmount,
                CreatedAt = dMTransactionEntityDomain.CreatedAt,
                FinancialMonth = dMTransactionEntityDomain.FinancialMonth,
                FinancialYear = dMTransactionEntityDomain.FinancialYear,
                Fund = dMTransactionEntityDomain.Fund,
                HousingBenefitAmount = dMTransactionEntityDomain.HousingBenefitAmount,
                IdDynamodb = dMTransactionEntityDomain.IdDynamodb,
                IsLoaded = dMTransactionEntityDomain.IsLoaded,
                IsSuspense = dMTransactionEntityDomain.IsSuspense,
                IsTransformed = dMTransactionEntityDomain.IsTransformed,
                PaidAmount = dMTransactionEntityDomain.PaidAmount,
                PaymentReference = dMTransactionEntityDomain.PaymentReference,
                PeriodNo = dMTransactionEntityDomain.PeriodNo,
                Person = dMTransactionEntityDomain.Person,
                SuspenseResolutionInfo = dMTransactionEntityDomain.SuspenseResolutionInfo,
                TargetId = dMTransactionEntityDomain.TargetId,
                TargetType = dMTransactionEntityDomain.TargetType,
                TransactionAmount = dMTransactionEntityDomain.TransactionAmount,
                TransactionDate = dMTransactionEntityDomain.TransactionDate,
                TransactionSource = dMTransactionEntityDomain.TransactionSource,
                TransactionType = dMTransactionEntityDomain.TransactionType
            };
        }

        //public static DMTransactionEntity ToDomain(this DMTransactionEntityUpdateRequest DMTransactionEntityUpdateRequest)
        //{
        //    return DMTransactionEntityUpdateRequest == null ? null : new DMTransactionEntity
        //    {
        //        DynamoDbEntityName = DMTransactionEntityUpdateRequest.DynamoDbEntity,
        //        ExpectedRowsToMigrate = DMTransactionEntityUpdateRequest.ExpectedRowsToMigrate,
        //        ActualRowsMigrated = DMTransactionEntityUpdateRequest.ActualRowsMigrated,
        //        StartRowId = DMTransactionEntityUpdateRequest.StartRowId,
        //        EndRowId = DMTransactionEntityUpdateRequest.EndRowId,
        //        LastRunDate = DMTransactionEntityUpdateRequest.LastRunDate,
        //        LastRunStatus = DMTransactionEntityUpdateRequest.LastRunStatus,
        //        UpdatedAt = DateTime.UtcNow
        //    };
        //}

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
