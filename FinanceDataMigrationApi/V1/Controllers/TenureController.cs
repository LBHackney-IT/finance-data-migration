using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.Tenure.Domain;
using Microsoft.VisualBasic;

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
        private readonly ITenureSaveToSqlUseCase _saveToSqlUseCase;
        private readonly ITenureGetLastHintUseCase _getLastHintUseCase;

        public TenureController(IGetTenureByPrnUseCase tenureByPrnUseCase
            , ITenureBatchInsertUseCase batchInsertUseCase
            , ITenureGetAllUseCase tenureGetAllUseCase
            , ITenureSaveToSqlUseCase saveToSqlUseCase
            , ITenureGetLastHintUseCase getLastHintUseCase)
        {
            _tenureByPrnUseCase = tenureByPrnUseCase;
            _batchInsertUseCase = batchInsertUseCase;
            _tenureGetAllUseCase = tenureGetAllUseCase;
            _saveToSqlUseCase = saveToSqlUseCase;
            _getLastHintUseCase = getLastHintUseCase;
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
        [Route("download-all")]
        public async Task<IActionResult> GetAll()
        {
            do
            {
                var lastKey = await _getLastHintUseCase.ExecuteAsync().ConfigureAwait(false);
                Dictionary<string, AttributeValue> lastEvaluatedKey = new Dictionary<string, AttributeValue>
                {
                    {"id",new AttributeValue{S = lastKey.ToString()}}
                };
                var response = await _tenureGetAllUseCase.ExecuteAsync(lastEvaluatedKey).ConfigureAwait(false);
                lastEvaluatedKey = response.LastKey;
                if (response.TenureInformation.Count == 0)
                    break;
                await _saveToSqlUseCase.ExecuteAsync(response.LastKey.Count > 0 ? lastEvaluatedKey["id"].S : lastKey.ToString(),
                    response.TenureInformation.ToXElement()).ConfigureAwait(false);

                if (response.LastKey.Count == 0)
                    break;

            } while (true);
            return Ok("Done");
        }

        [HttpGet]
        [Route("test-all")]
        public async Task<IActionResult> TestAll()
        {
            var response = await _tenureGetAllUseCase.ExecuteAsync(null).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
