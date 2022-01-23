using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/asset")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AssetController : BaseController
    {
        private readonly IAssetGetAllUseCase _getAllUseCase;
        private readonly IAssetSaveToSqlUseCase _assetSaveToSqlUseCase;
        private readonly IAssetGetLastHintUseCase _assetGetLastHintUseCase;

        public AssetController(IAssetGetAllUseCase getAllUseCase, IAssetSaveToSqlUseCase assetSaveToSqlUseCase,IAssetGetLastHintUseCase assetGetLastHintUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _assetSaveToSqlUseCase = assetSaveToSqlUseCase;
            _assetGetLastHintUseCase = assetGetLastHintUseCase;
        }

        [Route("download-all")]
        [HttpGet]
        public async Task<IActionResult> SaveAllInInterimFinanceSystem()
        {
            try
            {
                do
                {
                    var lastHint = await _assetGetLastHintUseCase.ExecuteAsync().ConfigureAwait(false);
                    var response = await _getAllUseCase.ExecuteAsync(lastHint==Guid.Empty?"": lastHint.ToString()).ConfigureAwait(false);
                    var result = response.Results.Assets.ToXElement();
                    if(response.lastHitId==null || response.Results.Assets.Count == 0)
                        break;
                    lastHint = Guid.Parse(response.lastHitId);
                    await _assetSaveToSqlUseCase.ExecuteAsync(lastHint.ToString(), result).ConfigureAwait(false);
                } while (true);

            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok("Operation done successfully");
        }
    }
}
