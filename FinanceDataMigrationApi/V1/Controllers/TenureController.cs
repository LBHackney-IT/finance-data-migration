using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenure")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenureController : BaseController
    {
        private readonly IGetTenureByPrnUseCase _tenureByPrnUseCase;

        public TenureController(IGetTenureByPrnUseCase tenureByPrnUseCase)
        {
            _tenureByPrnUseCase = tenureByPrnUseCase;
        }

        [HttpGet("{prn}")]
        public async Task<IActionResult> Get(string prn)
        {
            if (prn == null)
                return BadRequest($"{nameof(prn)} shouldn't be null.");
            if (string.IsNullOrEmpty(prn))
                return BadRequest($"{nameof(prn)} cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(prn))
                return BadRequest($"{nameof(prn)} cannot be null or whitespace.");

            var result = await _tenureByPrnUseCase.ExecuteAsync(prn).ConfigureAwait(false);
            if (result.Count == 0)
                return NotFound(prn);

            return Ok(result);
        }
    }
}
