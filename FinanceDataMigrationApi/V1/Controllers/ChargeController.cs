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
    [Route("api/v1/charge")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ChargeController : BaseController
    {
        readonly IExtractChargeEntityUseCase _extractChargeEntityUseCase;
        readonly ILoadChargeEntityUseCase _loadChargeEntityUseCase;
        readonly IChargeBatchInsertUseCase _batchInsertUseCase;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("extract")]
        public async Task<IActionResult> ExtractChargeEntity()
        {
            // Truncate should be removed from the extract stored procedure
            // The guid should be created in stored procedure to keep duplication check rule
            // Extract procedure should append new item's to the current list
            // Extract procedure should be executed asynchronously
            var runExtractChargeEntity = await _extractChargeEntityUseCase.ExecuteAsync().ConfigureAwait(false);

            if (runExtractChargeEntity.Continue == false)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.InternalServerError,
                    "Extract Charge Entity Task Failed!!"));
            }

            return Ok("Extracted successfully.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">Dynamodb scan limit.</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("load{count}")]
        public async Task<IActionResult> LoadChargeEntity(int count)
        {
            var runLoadChargeEntity = await _loadChargeEntityUseCase.ExecuteAsync(count).ConfigureAwait(false);
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
