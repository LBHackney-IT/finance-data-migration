using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Newtonsoft.Json;
using TargetType = Hackney.Shared.HousingSearch.Domain.Transactions.TargetType;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class TransactionFactory
    {
        public static Transaction ToTransactionRequest(this DmTransaction dMTransactionEntityDomain)
        {
            return dMTransactionEntityDomain == null
                ? null
                : new Transaction
                {
                    Id = dMTransactionEntityDomain.IdDynamodb,
                    BalanceAmount = dMTransactionEntityDomain.BalanceAmount ?? 0,
                    BankAccountNumber = dMTransactionEntityDomain.BankAccountNumber,
                    ChargedAmount = dMTransactionEntityDomain.ChargedAmount ?? 0,
                    Fund = dMTransactionEntityDomain.Fund,
                    HousingBenefitAmount = dMTransactionEntityDomain.HousingBenefitAmount ?? 0,
                    IsSuspense = dMTransactionEntityDomain.IsSuspense,
                    PaidAmount = dMTransactionEntityDomain.PaidAmount ?? 0,
                    PaymentReference = dMTransactionEntityDomain.PaymentReference,
                    PeriodNo = (short) dMTransactionEntityDomain.PeriodNo,
                    Sender = dMTransactionEntityDomain.Sender == null ? null : JsonConvert.DeserializeObject<Sender>(dMTransactionEntityDomain.Sender),
                    Address = null,
                    TargetId = dMTransactionEntityDomain.TargetId,
                    TargetType = Enum.Parse<TargetType>(dMTransactionEntityDomain.TargetType),
                    TransactionAmount = dMTransactionEntityDomain.TransactionAmount,
                    TransactionDate = dMTransactionEntityDomain.TransactionDate,
                    TransactionSource = dMTransactionEntityDomain.TransactionSource,
                    TransactionType = dMTransactionEntityDomain.TransactionType.TransactionTypeEnumValue(),
                    FinancialYear = (short) dMTransactionEntityDomain.FinancialYear,
                    FinancialMonth = (short) dMTransactionEntityDomain.FinancialMonth,
                    CreatedAt = dMTransactionEntityDomain.CreatedAt.UtcDateTime,
                    CreatedBy = dMTransactionEntityDomain.CreatedBy,
                    LastUpdatedAt = dMTransactionEntityDomain.LastUpdatedAt.UtcDateTime,
                    LastUpdatedBy = dMTransactionEntityDomain.LastUpdatedBy
                };

        }

        public static List<Transaction> ToTransactionRequestList(this IList<DmTransaction> transactions)
        {
            //return transactions.Select(item => item.ToTransactionRequest()).ToList();
            var transactionRequestList = transactions.Select(item => item.ToTransactionRequest()).ToList();
            return transactionRequestList;
        }

        public static Dictionary<string, AttributeValue> ToQueryRequest(this DmTransaction transaction)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = transaction.Id.ToString()}},
                {"address", new AttributeValue {S = transaction.Address}},
                {"balance_amount", new AttributeValue {N = transaction.BalanceAmount.ToString()}},
                {"bank_account_number", new AttributeValue {S = transaction.BankAccountNumber}},
                {"charged_amount", new AttributeValue {N = transaction.ChargedAmount.ToString()}},
                {"financial_month", new AttributeValue {N = transaction.FinancialMonth.ToString()}},
                {"financial_year", new AttributeValue {N = transaction.FinancialYear.ToString()}},
                {"fund", new AttributeValue {S = transaction.Fund}},
                {"housing_benefit_amount", new AttributeValue {N = transaction.HousingBenefitAmount.ToString()}},
                {"paid_amount", new AttributeValue {N = transaction.PaidAmount.ToString()}},
                {"payment_reference", new AttributeValue {S = transaction.PaymentReference}},
                {"period_no", new AttributeValue {N = transaction.PeriodNo.ToString("####")}},
                {"target_id", new AttributeValue {S = transaction.TargetId.ToString()}},
                {"is_suspense", new AttributeValue {S = transaction.IsSuspense.ToString()}},
                {"transaction_amount", new AttributeValue {N = transaction.TransactionAmount.ToString("F")}},
                {"transaction_date", new AttributeValue {S = transaction.TransactionDate.ToString("F")}},
                {"transaction_source", new AttributeValue {S = transaction.TransactionSource}},
                {"transaction_type", new AttributeValue {S = transaction.TransactionType.ToString()}},
                /*{
                    "sender",
                    new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            {"id", new AttributeValue {S = transaction.Sender.Id.ToString()}},
                            {"fullName", new AttributeValue {S = transaction.Sender.FullName}}
                        }
                    }
                },
                {
                    "suspense_resolution_info",
                    new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                        {
                            {"isConfirmed",new AttributeValue {BOOL = transaction.SuspenseResolutionInfo.IsConfirmed}},
                            {"isApproved",new AttributeValue {BOOL = transaction.SuspenseResolutionInfo.IsApproved}},
                            {"note", new AttributeValue {S = transaction.SuspenseResolutionInfo.Note}},
                            {"resolutionDate",new AttributeValue{S = transaction.SuspenseResolutionInfo.ResolutionDate.ToString()}}
                        }
                    }
                },*/
                {"created_at", new AttributeValue {S = transaction.CreatedAt.ToString("F")}},
                {"created_by", new AttributeValue {S = transaction.CreatedBy}},
                {"last_updated_at", new AttributeValue {S = transaction.LastUpdatedAt.ToString()}},
                {"last_updated_by", new AttributeValue {S = transaction.LastUpdatedBy}}
            };
        }

        public static DmTransactionDbEntity ToDatabase(this DmTransaction dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new DmTransactionDbEntity()
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
                IsIndexed = dMTransactionEntity.IsIndexed,
                IsSuspense = dMTransactionEntity.IsSuspense,
                PaidAmount = dMTransactionEntity.PaidAmount ?? 0,
                PaymentReference = dMTransactionEntity.PaymentReference,
                PeriodNo = dMTransactionEntity.PeriodNo,
                Sender = dMTransactionEntity.Sender,
                SuspenseResolutionInfo = dMTransactionEntity.SuspenseResolutionInfo,
                TargetId = dMTransactionEntity.TargetId,
                TargetType = dMTransactionEntity.TargetType,
                TransactionAmount = dMTransactionEntity.TransactionAmount,
                TransactionDate = dMTransactionEntity.TransactionDate,
                TransactionSource = dMTransactionEntity.TransactionSource,
                TransactionType = dMTransactionEntity.TransactionType,
                LastUpdatedAt = dMTransactionEntity.LastUpdatedAt.UtcDateTime,
                LastUpdatedBy = dMTransactionEntity.LastUpdatedBy,
                CreatedBy = dMTransactionEntity.CreatedBy,
                MigrationStatus = dMTransactionEntity.MigrationStatus,
                Address = dMTransactionEntity.Address
            };
        }

        public static DmTransaction ToDomain(this DmTransactionDbEntity dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new DmTransaction
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
                IsIndexed = dMTransactionEntity.IsIndexed,
                MigrationStatus = dMTransactionEntity.MigrationStatus,
                IsSuspense = dMTransactionEntity.IsSuspense,
                PaidAmount = dMTransactionEntity.PaidAmount,
                PaymentReference = dMTransactionEntity.PaymentReference,
                PeriodNo = (int) dMTransactionEntity.PeriodNo,
                Sender = dMTransactionEntity.Sender,
                SuspenseResolutionInfo = dMTransactionEntity.SuspenseResolutionInfo,
                TargetId = dMTransactionEntity.TargetId,
                TargetType = dMTransactionEntity.TargetType,
                TransactionAmount = dMTransactionEntity.TransactionAmount,
                TransactionDate = dMTransactionEntity.TransactionDate,
                TransactionSource = dMTransactionEntity.TransactionSource,
                TransactionType = dMTransactionEntity.TransactionType,
                LastUpdatedAt = dMTransactionEntity.LastUpdatedAt.UtcDateTime,
                LastUpdatedBy = dMTransactionEntity.LastUpdatedBy,
                CreatedBy = dMTransactionEntity.CreatedBy,
                Address = dMTransactionEntity.Address
            };
        }

        public static List<DmTransaction> ToDomain(this IList<DmTransactionDbEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }

        public static List<DmTransactionDbEntity> ToDatabase(this IList<DmTransaction> dMTransactionEntityDomainItems)
        {
            return dMTransactionEntityDomainItems.Select(p => p.ToDatabase()).ToList();
        }
    }
}
