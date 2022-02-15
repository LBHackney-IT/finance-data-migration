using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Infrastructure.Entities;
using FinanceDataMigrationApi.V1.Infrastructure.Extensions;
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
                    CreatedAt = dMTransactionEntityDomain.CreatedAt ?? DateTime.Now,
                    CreatedBy = dMTransactionEntityDomain.CreatedBy,
                    LastUpdatedAt = dMTransactionEntityDomain.LastUpdatedAt ?? DateTime.Now,
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

            Dictionary<string, AttributeValue> query = new Dictionary<string, AttributeValue>();

            query.PureAdd("id", new AttributeValue { S = transaction.IdDynamodb.ToString() });
            query.PureAdd("address", new AttributeValue { S = transaction.Address });
            query.PureAdd("balance_amount", new AttributeValue { N = transaction.BalanceAmount?.ToString() ?? "0" });
            query.PureAdd("bank_account_number", new AttributeValue { S = transaction.BankAccountNumber ?? "NA" });
            query.PureAdd("charged_amount", new AttributeValue { N = transaction.ChargedAmount?.ToString() ?? "0" });
            query.PureAdd("financial_month", new AttributeValue { N = transaction.FinancialMonth.ToString() });
            query.PureAdd("financial_year", new AttributeValue { N = transaction.FinancialYear.ToString() });
            query.PureAdd("fund", new AttributeValue { S = transaction.Fund ?? "NA" });
            query.PureAdd("housing_benefit_amount", new AttributeValue { N = transaction.HousingBenefitAmount?.ToString() ?? "0" });
            query.PureAdd("paid_amount", new AttributeValue { N = transaction.PaidAmount?.ToString() ?? "0" });
            query.PureAdd("payment_reference", new AttributeValue { S = transaction.PaymentReference });
            query.PureAdd("period_no", new AttributeValue { N = transaction.PeriodNo.ToString("####") ?? "0" });
            query.PureAdd("target_id", new AttributeValue { S = transaction.TargetId.ToString() });
            query.PureAdd("is_suspense", new AttributeValue { S = transaction.IsSuspense.ToString() });
            query.PureAdd("transaction_amount", new AttributeValue { N = transaction.TransactionAmount.ToString("F") ?? "0" });
            query.PureAdd("transaction_date", new AttributeValue { S = transaction.TransactionDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            query.PureAdd("transaction_source", new AttributeValue { S = transaction.TransactionSource });
            query.PureAdd("transaction_type", new AttributeValue { S = transaction.TransactionType.Trim().Replace(@"\", "").Replace("/", "").Replace(" ", "").Trim() });
            query.PureAdd("created_at", new AttributeValue { S = transaction.CreatedAt?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            query.PureAdd("created_by", new AttributeValue { S = transaction.CreatedBy });
            query.PureAdd("last_updated_at", new AttributeValue { S = transaction.LastUpdatedAt?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") });
            query.PureAdd("last_updated_by", new AttributeValue { S = transaction.LastUpdatedBy });
            query.PureAdd("sort_code", new AttributeValue { S = "NA" });
            query.PureAdd("target_type", new AttributeValue { S = "Tenure" });
            /*query.PureAdd("sender", new AttributeValue { S = "" });*/
            return query;
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
                LastUpdatedAt = dMTransactionEntity.LastUpdatedAt,
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
                LastUpdatedAt = dMTransactionEntity.LastUpdatedAt,
                LastUpdatedBy = dMTransactionEntity.LastUpdatedBy,
                CreatedBy = dMTransactionEntity.CreatedBy,
                Address = dMTransactionEntity.Address
            };
        }

        public static List<DmTransaction> ToDomains(this IList<DmTransactionDbEntity> databaseEntity)
        {
            return databaseEntity.Select(p => p.ToDomain()).ToList();
        }

        public static List<DmTransactionDbEntity> ToDatabases(this IList<DmTransaction> dMTransactionEntityDomainItems)
        {
            return dMTransactionEntityDomainItems.Select(p => p.ToDatabase()).ToList();
        }
    }
}
