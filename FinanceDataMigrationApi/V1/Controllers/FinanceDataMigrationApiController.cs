using FinanceDataMigrationApi.V1.Boundary.Request;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/migrationruns")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class FinanceDataMigrationApiController : BaseController
    {
        private readonly IGetMigrationRunByIdUseCase _getByIdUseCase;
        private readonly IGetAllUseCase _getAllUseCase;
        private readonly IGetMigrationRunByEntityNameUseCase _getMigrationRunByEntityNameUseCase;
        private readonly IUpdateUseCase _updateUseCase;

        public FinanceDataMigrationApiController(
            IGetMigrationRunByIdUseCase getByIdUseCase,
            IGetAllUseCase getAllUseCase,
            IGetMigrationRunByEntityNameUseCase getMigrationRunByEntityNameUseCase,
            IUpdateUseCase updateUseCase)
        {
            _getAllUseCase = getAllUseCase;
            _getByIdUseCase = getByIdUseCase;
            _updateUseCase = updateUseCase;
            _getMigrationRunByEntityNameUseCase = getMigrationRunByEntityNameUseCase;
        }

        [ProducesResponseType(typeof(MigrationRunResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "The id can not be empty!"));
            }

            var migrationRun = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (migrationRun == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "The MigrationRun by provided id not found!"));
            }

            return Ok(migrationRun);
        }

        [ProducesResponseType(typeof(MigrationRunResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetByEntityName([FromQuery] string entityName)
        {
            if (entityName == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "The entityName can not be empty. Supply entityName"));
            }

            var migrationRun = await _getMigrationRunByEntityNameUseCase.ExecuteAsync(entityName).ConfigureAwait(false);

            if (migrationRun == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "The MigrationRun by provided entityName not found!"));
            }

            return Ok(migrationRun);
        }



        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(MigrationRunResponseList), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        //[Route("{yourId}")]
        [Route("all")]

        public async Task<IActionResult> ListAllMigrationRuns()
        {
            var migrationsRuns = await _getAllUseCase.ExecuteAsync().ConfigureAwait(false);

            if (migrationsRuns == null)
            {
                return NotFound();
            }
            return Ok(migrationsRuns);
        }

        [ProducesResponseType(typeof(MigrationRunResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] MigrationRunUpdateRequest migrationRunUpdateRequest)
        {
            if (migrationRunUpdateRequest == null)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, "MigrationRun model cannot be null!"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseErrorResponse((int) HttpStatusCode.BadRequest, GetErrorMessage(ModelState)));
            }
            var result = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound(new BaseErrorResponse((int) HttpStatusCode.NotFound, "No MigrationRun by provided Id cannot be found!"));
            }

            var response = await _updateUseCase.ExecuteAsync(migrationRunUpdateRequest, id).ConfigureAwait(false);

            return Ok(response);
        }

        ///// <summary>
        ///// ...
        ///// </summary>
        ///// <response code="200">...</response>
        ///// <response code="404">No ? found for the specified ID</response>
        //[ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
        //[HttpGet]
        //[LogCall(LogLevel.Information)]
        ////TODO: rename to match the identifier that will be used
        //[Route("{yourId}")]
        //public IActionResult ViewRecord(int yourId)
        //{
        //    return Ok(_getByIdUseCase.Execute(yourId));
        //}
    }
}
