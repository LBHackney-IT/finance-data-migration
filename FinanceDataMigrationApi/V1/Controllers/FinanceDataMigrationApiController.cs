using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/data-migration")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class FinanceDataMigrationApiController : BaseController
    {

#if DEBUG 
        private readonly IExtractTransactionEntityUseCase _extractTransactionEntityUseCase;
        private readonly ITransformTransactionEntityUseCase _transformTransactionEntityUse;
        private readonly ILoadTransactionEntityUseCase _loadTransactionEntityUseCase;

        public FinanceDataMigrationApiController(
            IExtractTransactionEntityUseCase extractTransactionEntityUseCase,
            ITransformTransactionEntityUseCase transformTransactionEntityUseCase,
            ILoadTransactionEntityUseCase loadTransactionEntityUseCase)
        {
            _extractTransactionEntityUseCase = extractTransactionEntityUseCase;
            _transformTransactionEntityUse = transformTransactionEntityUseCase;
            _loadTransactionEntityUseCase = loadTransactionEntityUseCase;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("transaction-entity/extract")]
        public async Task<IActionResult> ExtractTransactionEntity()
        {
            var runExtractTransactionEntity = await _extractTransactionEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Extract Transaction Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("transaction-entity/transform")]
        public async Task<IActionResult> TransformTransactionEntity()
        {
            var runExtractTransactionEntity = await _transformTransactionEntityUse.ExecuteAsync().ConfigureAwait(false);

            if (runExtractTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Extract Transaction Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("transaction-entity/load")]
        public async Task<IActionResult> LoadTransactionEntity()
        {
            var runLoadTransactionEntity = await _loadTransactionEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runLoadTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Load Transaction Entity Task Failed!!"));
            }

            return Ok();
        }
#endif
    }
}
