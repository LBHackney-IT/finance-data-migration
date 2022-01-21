using Microsoft.AspNetCore.Mvc;
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
        private readonly ILoadTenuresUseCase _loadTenuresUseCase;

        public TenureController(IGetTenureByPrnUseCase tenureByPrnUseCase, ILoadTenuresUseCase loadTenuresUseCase)
        {
            _tenureByPrnUseCase = tenureByPrnUseCase;
            _loadTenuresUseCase = loadTenuresUseCase;
        }

        [HttpGet("{prn}")]
        public async Task<IActionResult> Get(string prn)
        {
            if (prn == null)
            {
                return BadRequest($"{nameof(prn)} should be provided.");
            }

            var result = await _tenureByPrnUseCase.ExecuteAsync(prn).ConfigureAwait(false);

            if (result.Count == 0)
            {
                return NotFound(prn);
            }

            return Ok(result);
        }

        /// <summary>
        /// This endpoint will load all Tenures from HousingSearchAPI source and save it in DM SQL database
        /// </summary>
        /// <returns></returns>
        [HttpPost("load")]
        public async Task<IActionResult> LoadToTemporaryStorageAsync()
        {
            await _loadTenuresUseCase.ExecuteAsync().ConfigureAwait(false);

            return Ok();
        }
    }
}
