using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/asset")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AssetController:BaseController
    {
        private readonly IAssetGetAllUseCase _getAllUseCase;

        public AssetController(IAssetGetAllUseCase getAllUseCase)
        {
            _getAllUseCase = getAllUseCase;
        }

        [Route("download-all")]
        [HttpGet]
        public async Task<IActionResult> SaveAllInInterimSinanceSystem()
        {
            var response = await _getAllUseCase.ExecuteAsync("").ConfigureAwait(false);
            return Ok(response.Results.Assets.ToXElement());
        }
    }
}
