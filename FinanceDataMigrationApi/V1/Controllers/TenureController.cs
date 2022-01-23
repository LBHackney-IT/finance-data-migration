using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Hackney.Shared.Tenure.Domain;
using Microsoft.VisualBasic;
using Nest;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/tenure")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TenureController : BaseController
    {
        private readonly IGetTenureByPrnUseCase _tenureByPrnUseCase;
        private readonly ITenureBatchInsertUseCase _batchInsertUseCase;
        private readonly ITenureGetAllUseCase _tenureGetAllUseCase;

        public TenureController(IGetTenureByPrnUseCase tenureByPrnUseCase,ITenureBatchInsertUseCase batchInsertUseCase,ITenureGetAllUseCase tenureGetAllUseCase)
        {
            _tenureByPrnUseCase = tenureByPrnUseCase;
            _batchInsertUseCase = batchInsertUseCase;
            _tenureGetAllUseCase = tenureGetAllUseCase;
        }

        [HttpGet("{prn}")]
        public async Task<IActionResult> Get(string prn)
        {
            if (prn == null)
                return BadRequest($"{nameof(prn)} shouldn't be null.");
            if (string.IsNullOrEmpty(prn))
                return BadRequest($"{nameof(prn)} cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(prn))
                return BadRequest($"{nameof(prn)} cannot be null or whitespace.");

            var result = await _tenureByPrnUseCase.ExecuteAsync(prn).ConfigureAwait(false);
            if (result.Count == 0)
                return NotFound(prn);

            return Ok(result);
        }

        [Route("dummy-async")]
        [HttpPost]
        public async Task<IActionResult> DummyBatchInsertAsync(int count)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < count / 25; i++)
            {
                Fixture fixture = new Fixture();
                List<TenureInformation> transactions = fixture.CreateMany<TenureInformation>(25).ToList();
                tasks.Add(_batchInsertUseCase.ExecuteAsync(transactions));
            }
            DateTime startDateTime = DateTime.Now;
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return Ok($"Elapsed time: {DateAndTime.Now.Subtract(startDateTime).TotalSeconds}");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            TenurePaginationResponse response = new TenurePaginationResponse()
            {
                PaginationToken = "{}"
            };
            int index = 0;
            while ((response.TenureInformations?.Count??0)>0 || index++==0)
            {
                response = await _tenureGetAllUseCase.ExecuteAsync(response.PaginationToken).ConfigureAwait(false);
            }
            return Ok(response);
        }
    }
}
