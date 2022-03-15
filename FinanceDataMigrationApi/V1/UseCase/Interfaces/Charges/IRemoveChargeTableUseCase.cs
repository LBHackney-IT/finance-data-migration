using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges
{
    public interface IRemoveChargeTableUseCase
    {
        public Task<DeleteTableResponse> ExecuteAsync();
    }
}
