using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Transactions;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Transactions
{
    public class DeleteAllTransactionEntityUseCase : IDeleteAllTransactionEntityUseCase
    {
        private readonly ITransactionGateway _transactionGateway;

        public DeleteAllTransactionEntityUseCase(ITransactionGateway transactionGateway)
        {
            _transactionGateway = transactionGateway;
        }

        public async Task<StepResponse> ExecuteAsync()
        {
            var response = await _transactionGateway.DeleteAllTransactionAsync().ConfigureAwait(false);

            return new StepResponse { Continue = response, NextStepTime = DateTime.MaxValue };
        }
    }
}
