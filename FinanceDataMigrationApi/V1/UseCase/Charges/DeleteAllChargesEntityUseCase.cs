using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Charges;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.UseCase.Charges
{
    public class DeleteAllChargesEntityUseCase : IDeleteAllChargesEntityUseCase
    {

        private readonly IChargeGateway _chargeGateway;

        public DeleteAllChargesEntityUseCase(IChargeGateway chargeGateway)
        {

            _chargeGateway = chargeGateway;
        }
        public async Task<StepResponse> ExecuteAsync()
        {
            var response = await _chargeGateway.DeleteAllChargesAsync().ConfigureAwait(false);

            return new StepResponse { Continue = response, NextStepTime = DateTime.MaxValue };
        }
    }
}
