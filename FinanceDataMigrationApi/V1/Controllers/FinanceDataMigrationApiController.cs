using FinanceDataMigrationApi.V1.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

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
        private readonly IIndexTransactionEntityUseCase _indexTransactionEntityUseCase;

        public FinanceDataMigrationApiController(
            IExtractTransactionEntityUseCase extractTransactionEntityUseCase,
            ITransformTransactionEntityUseCase transformTransactionEntityUseCase,
            ILoadTransactionEntityUseCase loadTransactionEntityUseCase,
            IIndexTransactionEntityUseCase indexTransactionEntityUseCase)
        {
            _extractTransactionEntityUseCase = extractTransactionEntityUseCase;
            _transformTransactionEntityUse = transformTransactionEntityUseCase;
            _loadTransactionEntityUseCase = loadTransactionEntityUseCase;
            _indexTransactionEntityUseCase = indexTransactionEntityUseCase;
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

            return Ok("Transaction Entities Extracted Successfully");
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

            return Ok("Transaction Entities Loaded Successfully");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("transaction-entity/index-all")]
        public async Task<IActionResult> IndexTransactionEntity()
        {
            var runLoadTransactionEntity = await _indexTransactionEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runLoadTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Index Transaction Entity Task Failed!!"));
            }

            return Ok("Transaction Entities Indexed Successfully");
        }
#endif
    }
}
