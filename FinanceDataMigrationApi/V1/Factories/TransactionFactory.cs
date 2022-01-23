using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Newtonsoft.Json;
using TargetType = Hackney.Shared.HousingSearch.Domain.Transactions.TargetType;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class TransactionFactory
    {
        public static Transaction ToTransactionRequest(this DMTransactionEntityDomain dMTransactionEntityDomain)
        {
            try
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
                        Sender =
                            JsonConvert.DeserializeObject<Sender>(dMTransactionEntityDomain.Person), // TODO FIX PERSON
                        Address = null,
                        TargetId = dMTransactionEntityDomain.TargetId,
                        TargetType = dMTransactionEntityDomain.TargetType.TargetTypeEnumValue(),
                        TransactionAmount = dMTransactionEntityDomain.TransactionAmount,
                        TransactionDate = dMTransactionEntityDomain.TransactionDate,
                        TransactionSource = dMTransactionEntityDomain.TransactionSource,
                        TransactionType = dMTransactionEntityDomain.TransactionType.TransactionTypeEnumValue(),
                        FinancialYear = (short)dMTransactionEntityDomain.FinancialYear,
                        FinancialMonth = (short)dMTransactionEntityDomain.FinancialMonth
                    };
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }

        }

        public static List<Transaction> ToTransactionRequestList(this IList<DMTransactionEntityDomain> transactions)
        {
            //return transactions.Select(item => item.ToTransactionRequest()).ToList();
            var transactionRequestList = transactions.Select(item => item.ToTransactionRequest()).ToList();
            return transactionRequestList;
        }

        public static Dictionary<string, AttributeValue> ToQueryRequest(this Transaction transaction)
        {
            return new Dictionary<string, AttributeValue>()
            {
                {"id", new AttributeValue {S = transaction.Id.ToString()}},
                {"address", new AttributeValue {S = transaction.Address}},
                {"balance_amount", new AttributeValue {N = transaction.BalanceAmount.ToString("F")}},
                {"bank_account_number", new AttributeValue {S = transaction.BankAccountNumber.ToString()}},
                {"charged_amount", new AttributeValue {N = transaction.ChargedAmount.ToString("F")}},
                {"financial_month", new AttributeValue {N = transaction.FinancialMonth.ToString()}},
                {"financial_year", new AttributeValue {N = transaction.FinancialYear.ToString()}},
                {"fund", new AttributeValue {S = transaction.Fund}},
                {"housing_benefit_amount", new AttributeValue {N = transaction.HousingBenefitAmount.ToString("F")}},
                {"paid_amount", new AttributeValue {N = transaction.PaidAmount.ToString("F")}},
                {"payment_reference", new AttributeValue {S = transaction.PaymentReference}},
                {"period_no", new AttributeValue {N = transaction.PeriodNo.ToString()}},
                {"target_id", new AttributeValue {S = transaction.TargetId.ToString()}},
                {"is_suspense", new AttributeValue {S = transaction.IsSuspense.ToString()}},
                {"transaction_amount", new AttributeValue {N = transaction.TransactionAmount.ToString("F")}},
                {"transaction_date", new AttributeValue {S = transaction.TransactionDate.ToString("F")}},
                {"transaction_source", new AttributeValue {S = transaction.TransactionSource}},
                {"transaction_type", new AttributeValue {S = transaction.TransactionType.ToString()}},
                {
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
                },
                {"created_by", new AttributeValue {S = transaction.CreatedBy}},
                {"last_updated_by", new AttributeValue {S = transaction.LastUpdatedBy}},
                {"last_updated_at", new AttributeValue {S = transaction.LastUpdatedAt.ToString()}},
                {"created_at", new AttributeValue {S = transaction.CreatedAt.ToString("F")}}
            };
        }
    }
}
