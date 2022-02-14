using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Handlers;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;

namespace FinanceDataMigrationApi.V1.UseCase.Accounts
{
    public class DeleteAllAccountEntityUseCase : IDeleteAllAccountEntityUseCase
    {
       
        private readonly IAccountsGateway _accountsGateway;

        public DeleteAllAccountEntityUseCase(IAccountsGateway accountsGateway)
        {
            
            _accountsGateway = accountsGateway;
        }
        public async Task<StepResponse> ExecuteAsync()
        {
            var response = await _accountsGateway.DeleteAllAccountAsync().ConfigureAwait(false);

            return new StepResponse { Continue = response, NextStepTime = DateTime.MaxValue };
        }
    }
}
