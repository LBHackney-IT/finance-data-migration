using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
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
    public class AccountDataMigrationController : BaseController
    {
        private readonly IIndexAccountEntityUseCase _indexAccountEntityUseCase;

        public AccountDataMigrationController(IIndexAccountEntityUseCase indexAccountEntityUseCase)
        {
            _indexAccountEntityUseCase = indexAccountEntityUseCase;
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
        }
    }
}
