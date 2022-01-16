using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
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
        private readonly ITransactionGateway _gateway;

        public TransactionController(ITransactionGateway gateway)
        {
            _gateway = gateway;
        }
        [Route("{transactions}")]
        [HttpPost]
        public async Task<IActionResult> BatchInsert(List<Transaction> transactions)
        {
            await _gateway.BatchInsert(transactions).ConfigureAwait(false);
            return Ok("True");
        }
    }
}
