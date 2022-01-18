using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charges")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ConsolidatedChargeController : BaseController
    {
        private readonly IGetConsolidatedChargesByIdUseCase _getChargesByIdUseCase;
        public ConsolidatedChargeController(IGetConsolidatedChargesByIdUseCase getChargesByIdUseCase)
        {
            _getChargesByIdUseCase = getChargesByIdUseCase;
        }

        [HttpGet("{targetId}")]
        public async Task<IActionResult> GetAllByTargetIdAsync(Guid targetId)
        {
            var consolidatedCharges = await _getChargesByIdUseCase.ExecuteAsync(targetId).ConfigureAwait(false);

            if (consolidatedCharges == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charges by provided type and targetId cannot be found!"));
            }

            return Ok(consolidatedCharges);
        }
    }
}
