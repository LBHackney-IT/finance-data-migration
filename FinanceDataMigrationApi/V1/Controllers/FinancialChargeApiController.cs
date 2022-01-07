using System;
using System.Net;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/data-migration")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class FinancialChargeApiController : BaseController
    {
        private readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        private readonly ITransformChargeEntityUseCase _transformChargeEntityUseCase;
        //private readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;

        public FinancialChargeApiController(
            IExtractChargeEntityUseCase extractChargeEntityUseCase,
            ITransformChargeEntityUseCase transformChargeEntityUseCase
            //,ILoadChargeEntityUseCase loadChargeEntityUseCase
        )
        {
            _extractChargeEntityUseCase = extractChargeEntityUseCase;
            _transformChargeEntityUseCase = transformChargeEntityUseCase;
            //_loadChargeEntityUseCase = loadChargeEntityUseCase;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/extract")]
        public async Task<IActionResult> ExtractChargeEntity()
        {
            var runExtractChargeEntity = await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractChargeEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
                    "Extract Charge Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/transform")]
        public async Task<IActionResult> TransformChargeEntity()
        {
            var runExtractChargeEntity = await _transformChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractChargeEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
                    "Transform Charge Entity Task Failed!!"));
            }

            return Ok("Charge Entities Transformed Successfully");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/load")]
        public Task<IActionResult> LoadChargeEntity()
        {
            // var runLoadChargeEntity = await _loadChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);
            //
            // if (runLoadChargeEntity.Continue == false)
            // {
            //     return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
            //         "Load Charge Entity Task Failed!!"));
            // }
            //
            // return Ok("Charge Entities Loaded Successfully");

            throw new NotImplementedException();
        }
    }
}
