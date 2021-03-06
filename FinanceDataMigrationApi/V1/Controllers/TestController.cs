using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1/migration")]
    [ApiVersion("1.0")]
    public class TestController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> CallHandler()
        {
            await new Handler().IndexTransactions().ConfigureAwait(false);
            return Ok("Done");
        }
    }
}
