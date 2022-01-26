using System;
using System.Collections.Generic;
using System.Linq;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;

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
    }
}
