using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/data-migration")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeController : BaseController
    {
        private readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        private readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        private readonly IChargeBatchInsertUseCase _batchInsertUseCase;

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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/extract")]
        public IActionResult ExtractChargeEntity()
        {
            return Ok("Test");
            /*string step = "ExtractChargeEntity";
            try
            {
                step = "ExtractChargeEntity+_extractChargeEntityUseCase.ExecuteAsync()";
                var runExtractChargeEntity = await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

                if (runExtractChargeEntity.Continue == false)
                {
                    return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
                        "Extract Charge Entity Task Failed!!"));
                }

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest($"{exception} step:{step}");
                //throw new Exception($"{exception} step:{step}");
            }*/
        }

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        //[HttpGet]
        //[Route("charge-entity/transform")]
        //public async Task<IActionResult> TransformChargeEntity()
        //{
        //    var runExtractChargeEntity = await _transformChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

        //    if (runExtractChargeEntity.Continue == false)
        //    {
        //        return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
        //            "Transform Charge Entity Task Failed!!"));
        //    }

        //    return Ok("Charge Entities Transformed Successfully");
        //}

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("charge-entity/load")]
        public async Task<IActionResult> LoadChargeEntity()
        {

            var runLoadChargeEntity = await _loadChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);
            /*return Ok($"Elapsed time: {DateAndTime.Now.Subtract(startDateTime).TotalSeconds}");*/
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

        }
    }
}
