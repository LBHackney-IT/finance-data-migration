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
                    BalanceAmount = dMTransactionEntityDomain.BalanceAmount,
                    BankAccountNumber = dMTransactionEntityDomain.BankAccountNumber,
                    ChargedAmount = dMTransactionEntityDomain.ChargedAmount,
                    Fund = dMTransactionEntityDomain.Fund,
                    HousingBenefitAmount = dMTransactionEntityDomain.HousingBenefitAmount,
                    IsSuspense = dMTransactionEntityDomain.IsSuspense,
                    PaidAmount = dMTransactionEntityDomain.PaidAmount,
                    PaymentReference = dMTransactionEntityDomain.PaymentReference,
                    PeriodNo = (short) dMTransactionEntityDomain.PeriodNo,
                    Person = JsonConvert.DeserializeObject<Person>(dMTransactionEntityDomain.Person), // TODO FIX PERSON
                    //Person = new Person(), // TODO FIX PERSON
                    Address = null,
                    TargetId = dMTransactionEntityDomain.TargetId,
                    TargetType = dMTransactionEntityDomain.TargetType.TargetTypeEnumValue(),
                    TransactionAmount = dMTransactionEntityDomain.TransactionAmount,
                    TransactionDate = dMTransactionEntityDomain.TransactionDate,
                    TransactionSource = dMTransactionEntityDomain.TransactionSource,
                    TransactionType = dMTransactionEntityDomain.TransactionType.TransactionTypeEnumValue(),
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
