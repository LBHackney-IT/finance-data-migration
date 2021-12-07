using System.Collections.Generic;
using System.Linq;
using FinanceDataMigrationApi.V1.Domain;

namespace FinanceDataMigrationApi.V1.Factories
{
    public static class TransactionFactory
    {
        public static AddTransactionRequest ToTransactionRequest(this DMTransactionEntityDomain dMTransactionEntity)
        {
            return dMTransactionEntity == null ? null : new AddTransactionRequest
            {

                BalanceAmount = dMTransactionEntity.BalanceAmount,
                BankAccountNumber = dMTransactionEntity.BankAccountNumber,
                ChargedAmount = dMTransactionEntity.ChargedAmount,
                Fund = dMTransactionEntity.Fund,
                HousingBenefitAmount = dMTransactionEntity.HousingBenefitAmount,
                IsSuspense = dMTransactionEntity.IsSuspense,
                PaidAmount = dMTransactionEntity.PaidAmount,
                PaymentReference = dMTransactionEntity.PaymentReference,
                PeriodNo = dMTransactionEntity.PeriodNo,
                Person = dMTransactionEntity.Person,
                TargetId = dMTransactionEntity.TargetId,
                TargetType = dMTransactionEntity.TargetType,
                TransactionAmount = dMTransactionEntity.TransactionAmount,
                TransactionDate = dMTransactionEntity.TransactionDate,
                TransactionSource = dMTransactionEntity.TransactionSource,
                TransactionType = dMTransactionEntity.TransactionType,
            };
        }
        public static List<AddTransactionRequest> ToTransactionRequestList(this IList<DMTransactionEntityDomain> transactions)
        {
            return transactions.Select(item => item.ToTransactionRequest()).ToList();
        }
    }
}
