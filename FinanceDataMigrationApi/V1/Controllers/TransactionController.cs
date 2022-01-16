using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TransactionController : BaseController
    {
        private readonly IBatchInsertUseCase _batchInsertUseCase;

        public TransactionController(IBatchInsertUseCase batchInsertUseCase)
        {
            _batchInsertUseCase = batchInsertUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> BatchInsert(List<Transaction> transactions)
        {
            await _batchInsertUseCase.ExecuteAsync(transactions).ConfigureAwait(false);
            return Ok("True");
        }

        [Route("dummy")]
        [HttpPost]
        public async Task<IActionResult> DummyBatchInsert(int count)
        {
            for (int i = 0; i < 1000; i++)
            {
                Fixture fixture = new Fixture();
                List<Transaction> transactions = fixture.CreateMany<Transaction>(count).ToList();
                await _batchInsertUseCase.ExecuteAsync(transactions).ConfigureAwait(false); 
            }
            return Ok("True");
        }
    }
}
