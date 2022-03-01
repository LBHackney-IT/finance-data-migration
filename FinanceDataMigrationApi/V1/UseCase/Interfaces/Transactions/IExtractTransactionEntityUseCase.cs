using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions
{
    public interface IExtractTransactionEntityUseCase
    {
        public Task ExecuteAsync();
    }
}
