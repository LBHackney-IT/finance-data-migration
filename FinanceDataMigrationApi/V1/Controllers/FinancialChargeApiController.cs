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
        private readonly ITransformChargeEntityUseCase _transformChargeEntityUse;
        private readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        private readonly IIndexChargeEntityUseCase _indexChargeEntityUseCase;

        public FinancialChargeApiController(
            IExtractChargeEntityUseCase extractChargeEntityUseCase,
            ITransformChargeEntityUseCase transformChargeEntityUse,
            ILoadChargeEntityUseCase loadChargeEntityUseCase,
            IIndexChargeEntityUseCase indexChargeEntityUseCase
            )
        {
            _extractChargeEntityUseCase = extractChargeEntityUseCase;
            _transformChargeEntityUse = transformChargeEntityUse;
            _loadChargeEntityUseCase = loadChargeEntityUseCase;
            _indexChargeEntityUseCase = indexChargeEntityUseCase;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/extract")]
        public async Task<IActionResult> ExtractChargeEntity()
        {
            var runExtractTransactionEntity = await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Extract Charge Entity Task Failed!!"));
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
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/load")]
        public async Task<IActionResult> LoadChargeEntity()
        {
            throw new NotImplementedException();
        }
    }
}
