using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories.Interface;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.HousingSearch.Gateways.Models.Transactions;
using TargetType = Hackney.Shared.HousingSearch.Domain.Transactions.TargetType;

namespace FinanceDataMigrationApi.V1.Factories
{
    public class TransactionsQueryableFactory : IQueryableFactory<DmTransaction, QueryableTransaction>
    {
        private readonly DmTransaction _transaction;

        public TransactionsQueryableFactory(DmTransaction transaction)
        {
            _transaction = transaction;
        }

        public QueryableTransaction ToQuaryable()
        {
            /*if (transaction is null) throw new ArgumentNullException(nameof(transaction));*/

            return new QueryableTransaction()
            {
                Id = _transaction.IdDynamodb,
                Address = _transaction.Address,
                BalanceAmount = (decimal) (_transaction.BalanceAmount ?? 0),
                BankAccountNumber = _transaction.BankAccountNumber,
                ChargedAmount = (decimal) (_transaction.ChargedAmount ?? 0),
                FinancialMonth = (short) _transaction.FinancialMonth,
                FinancialYear = (short) _transaction.FinancialYear,
                Fund = _transaction.Fund,
                HousingBenefitAmount = (decimal) (_transaction.HousingBenefitAmount ?? 0),
                PaidAmount = (decimal) (_transaction.PaidAmount ?? 0),
                PaymentReference = _transaction.PaymentReference,
                PeriodNo = (short) _transaction.PeriodNo,
                Sender = null,
                SuspenseResolutionInfo = /*_transaction.SuspenseResolutionInfo != null ? new QueryableSuspenseResolutionInfo()
                {
                    IsApproved = _transaction.SuspenseResolutionInfo.IsApproved,
                    IsConfirmed = _transaction.SuspenseResolutionInfo.IsConfirmed,
                    Note = _transaction.SuspenseResolutionInfo.Note,
                    ResolutionDate = _transaction.SuspenseResolutionInfo.ResolutionDate
                } :*/ null,
                TargetId = _transaction.TargetId,
                TargetType = Enum.Parse<TargetType>(_transaction.TargetType),
                TransactionAmount = _transaction.TransactionAmount,
                TransactionDate = _transaction.TransactionDate,
                TransactionSource = _transaction.TransactionSource,
                TransactionType = Enum.Parse<TransactionType>(_transaction.TransactionType),
                SortCode = null,//_transaction.SortCode,
                CreatedAt = (DateTime) (_transaction.CreatedAt ?? DateTime.UtcNow),
                CreatedBy = _transaction.CreatedBy,
                LastUpdatedAt = _transaction.LastUpdatedAt,
                LastUpdatedBy = _transaction.LastUpdatedBy
            };
        }
    }
}
