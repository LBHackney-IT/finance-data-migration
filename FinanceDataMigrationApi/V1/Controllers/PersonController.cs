using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/person")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PersonController : BaseController
    {
        private readonly IGetPersonByIdUseCase _personByIdUseCase;

        public PersonController(IGetPersonByIdUseCase personByIdUseCase)
        {
            _personByIdUseCase = personByIdUseCase;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest($"{nameof(id)} cannot be empty.");

            var result = await _personByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);
            if (result == null)
                return NotFound(id);
            return Ok(result);
        }
    }
}
