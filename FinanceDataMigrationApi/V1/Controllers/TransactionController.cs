using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using Hackney.Shared.HousingSearch.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace FinanceDataMigrationApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionBatchInsertUseCase _transactionBatchInsertUseCase;

        public TransactionController(ITransactionBatchInsertUseCase transactionBatchInsertUseCase)
        {
            _transactionBatchInsertUseCase = transactionBatchInsertUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> BatchInsert(List<Transaction> transactions)
        {
            await _transactionBatchInsertUseCase.ExecuteAsync(transactions).ConfigureAwait(false);
            return Ok("True");
        }

        [Route("dummy-sync")]
        [HttpPost]
        public async Task<IActionResult> DummyBatchInsertSync(int count)
        {
            double totlaSeconds = 0;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < count/25; i++)
            {
                Fixture fixture = new Fixture();
                List<Transaction> transactions = fixture.CreateMany<Transaction>(25).ToList();
                DateTime startDateTime = DateTime.Now;
                await _transactionBatchInsertUseCase.ExecuteAsync(transactions).ConfigureAwait(false);
                totlaSeconds += DateTime.Now.Subtract(startDateTime).TotalSeconds;
            }
            return Ok($"Elapsed time: {totlaSeconds}");
        }

        [Route("dummy-async")]
        [HttpPost]
        public async Task<IActionResult> DummyBatchInsertAsync(int count)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < count / 25; i++)
            {
                Fixture fixture = new Fixture();
                List<Transaction> transactions = fixture.CreateMany<Transaction>(25).ToList();
                tasks.Add(_transactionBatchInsertUseCase.ExecuteAsync(transactions));
            }
            DateTime startDateTime = DateTime.Now;
            await Task.WhenAll(tasks).ConfigureAwait(false);
            return Ok($"Elapsed time: {DateAndTime.Now.Subtract(startDateTime).TotalSeconds}");
        }
    }
}
