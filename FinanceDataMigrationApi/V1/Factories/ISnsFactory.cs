using FinanceDataMigrationApi.V1.Domain;
using Hackney.Shared.HousingSearch.Domain.Transactions;

namespace FinanceDataMigrationApi.V1.Factories
{
    public interface ISnsFactory
    {
        TransactionSns Create(Transaction transaction);

        TransactionSns Update(Transaction transaction);
    }
}
