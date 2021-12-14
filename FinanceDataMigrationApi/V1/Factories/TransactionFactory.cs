using System;
using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Handlers;
using Newtonsoft.Json;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class TransactionFactory
    {
        public static AddTransactionRequest ToTransactionRequest(this DMTransactionEntityDomain dMTransactionEntityDomain)
        {
            try
            {
                return dMTransactionEntityDomain == null ? null : new AddTransactionRequest
                {
                    BalanceAmount = dMTransactionEntityDomain.BalanceAmount?? 0,
                    BankAccountNumber = dMTransactionEntityDomain.BankAccountNumber,
                    ChargedAmount = dMTransactionEntityDomain.ChargedAmount?? 0,
                    Fund = dMTransactionEntityDomain.Fund,
                    HousingBenefitAmount = dMTransactionEntityDomain.HousingBenefitAmount?? 0,
                    IsSuspense = dMTransactionEntityDomain.IsSuspense,
                    PaidAmount = dMTransactionEntityDomain.PaidAmount?? 0,
                    PaymentReference = dMTransactionEntityDomain.PaymentReference,
                    PeriodNo = (short) dMTransactionEntityDomain.PeriodNo,
                    Person = JsonConvert.DeserializeObject<TransactionPerson>(dMTransactionEntityDomain.Person), // TODO FIX PERSON
                    Address = null,
                    TargetId = dMTransactionEntityDomain.TargetId,
                    TargetType = dMTransactionEntityDomain.TargetType.TargetTypeEnumValue(),
                    TransactionAmount = dMTransactionEntityDomain.TransactionAmount,
                    TransactionDate = dMTransactionEntityDomain.TransactionDate,
                    TransactionSource = dMTransactionEntityDomain.TransactionSource,
                    TransactionType = dMTransactionEntityDomain.TransactionType.TransactionTypeEnumValue(),
                    FinancialYear = dMTransactionEntityDomain.FinancialYear,
                    FinancialMonth = dMTransactionEntityDomain.FinancialMonth
                };
            }
            catch (Exception e)
            {
                LoggingHandler.LogError(e.Message);
                LoggingHandler.LogError(e.StackTrace);
                throw;
            }

        }
        public static List<AddTransactionRequest> ToTransactionRequestList(this IList<DMTransactionEntityDomain> transactions)
        {
            //return transactions.Select(item => item.ToTransactionRequest()).ToList();
            var transactionRequestList = transactions.Select(item => item.ToTransactionRequest()).ToList();  
            return transactionRequestList;
        }
    }
}
