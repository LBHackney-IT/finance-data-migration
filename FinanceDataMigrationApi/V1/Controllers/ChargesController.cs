using FinanceDataMigrationApi.V1.Boundary.Request;
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
    public class ChargesController : BaseController
    {
        private readonly IGetChargesByIdUseCase _getChargesByIdUseCase;
        public ChargesController(IGetChargesByIdUseCase getChargesByIdUseCase)
        {
            _getChargesByIdUseCase = getChargesByIdUseCase;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllAsync([FromQuery] Guid targetId)
        {
            var charges = await _getChargesByIdUseCase.ExecuteAsync(targetId).ConfigureAwait(false);

            if (charges == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No Charges by provided type and targetId cannot be found!"));
            }

            return Ok(charges);
        }
    }
}
