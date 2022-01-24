using System;
using System.Threading.Tasks;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.Tests.V1.Gateways
{
    public class DmTransactionEntityGatewayProxy : DMTransactionEntityGateway, IDMTransactionEntityGateway
    {
        public Task<int> NumberOfRowsExtractedResult { get; private set; }

        public DmTransactionEntityGatewayProxy(DbTransactionsContext context) : base(context)
        {
        }

        public new Task<int> ExtractAsync(DateTimeOffset? processingDate)
        {
            NumberOfRowsExtractedResult = base.ExtractAsync(processingDate);
            return NumberOfRowsExtractedResult;
        }

    }
}
