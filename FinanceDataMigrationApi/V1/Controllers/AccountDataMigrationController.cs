using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Nest;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/data-migration")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AccountDataMigrationController : BaseController
    {
        [HttpGet]
        [Route(("extract"))]
        public async Task<IActionResult> Extract()
        {
            Handler handler = new Handler();
            await handler.ExtractAccount().ConfigureAwait(false);
            return Ok("Done");
        }
        [HttpGet]
        [Route(("load"))]
        public async Task<IActionResult> Load()
        {
            Handler handler = new Handler();
            await handler.LoadAccount().ConfigureAwait(false);
            return Ok("Done");
        }

        /*private readonly IExtractAccountEntityUseCase _extractAccountEntityUseCase;
        private readonly ITransformAccountsUseCase _transformAccountsUseCase;
        private readonly ILoadAccountsUseCase _loadAccountsUseCase;
        private readonly IIndexAccountEntityUseCase _indexAccountEntityUseCase;

        public AccountDataMigrationController(
            IIndexAccountEntityUseCase indexAccountEntityUseCase,
            IExtractAccountEntityUseCase extractAccountEntityUseCase,
            ITransformAccountsUseCase transformAccountsUseCase,
            ILoadAccountsUseCase loadAccountsUseCase)
        {
            _indexAccountEntityUseCase = indexAccountEntityUseCase;
            _extractAccountEntityUseCase = extractAccountEntityUseCase;
            _transformAccountsUseCase = transformAccountsUseCase;
            _loadAccountsUseCase = loadAccountsUseCase;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("accounts-entity/extract")]
        public async Task<IActionResult> ExtractAccountEntity()
        {
            var runExtractTransactionEntity = await _extractAccountEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Extract Account Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("accounts-entity/transform")]
        public async Task<IActionResult> TransformAccountEntity()
        {
            var runTransformTransactionEntity = await _transformAccountsUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runTransformTransactionEntity.Continue == false)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Transform Account Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("accounts-entity/load")]
        public async Task<IActionResult> LoadAccountEntity()
        {
            var runLoadTransactionEntity = await _loadAccountsUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runLoadTransactionEntity.Continue == false)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Load Account Entity Task Failed!!"));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("accounts-entity/index-all")]
        public async Task<IActionResult> IndexAccountEntity()
        {
            var runLoadTransactionEntity = await _indexAccountEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runLoadTransactionEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Index Account Entity Task Failed!!"));
            }

            return Ok("Account Entities Indexed Successfully");
        }*/
    }
}
