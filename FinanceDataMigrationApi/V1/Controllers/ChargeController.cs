using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/charge")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeController : BaseController
    {

        [HttpGet]
        [Route("charge-load")]
        public async Task<IActionResult> Load()
        {
            Handler handler = new Handler();
            await handler.LoadCharge().ConfigureAwait(false);
            return Ok("Done");
        }

        [HttpGet]
        [Route("charge-extract")]
        public async Task<IActionResult> Extract()
        {
            Handler handler = new Handler();
            await handler.ExtractCharge().ConfigureAwait(false);
            return Ok("Done");
        }
        /*readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        readonly IChargeBatchInsertUseCase _batchInsertUseCase;

        public ChargeController(
            IExtractChargeEntityUseCase extractChargeEntityUseCase,
            ILoadChargeEntityUseCase loadChargeEntityUseCase,
            IChargeBatchInsertUseCase batchInsertUseCase
        )
        {
            _extractChargeEntityUseCase = extractChargeEntityUseCase;
            _loadChargeEntityUseCase = loadChargeEntityUseCase;
            _batchInsertUseCase = batchInsertUseCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("extract")]
        public async Task<IActionResult> ExtractChargeEntity()
        {
            await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            return Ok("Extracted successfully.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">Dynamodb scan limit.</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("load")]
        public async Task<IActionResult> LoadChargeEntity([FromQuery] int count)
        {
            var runLoadChargeEntity = await _loadChargeEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
            *//*return Ok($"Elapsed time: {DateAndTime.Now.Subtract(startDateTime).TotalSeconds}");*//*
            return Ok("Done");
            // var runLoadChargeEntity = await _loadChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);
            //
            // if (runLoadChargeEntity.Continue == false)
            // {
            //     return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
            //         "Load Charge Entity Task Failed!!"));
            // }
            //
            // return Ok("Charge Entities Loaded Successfully");

        }*/
    }
}
