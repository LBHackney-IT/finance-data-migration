using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure;

namespace FinanceDataMigrationApi.Tests.V1.Gateways
{
    public class DmTransactionEntityGatewayProxy : TransactionGateway, ITransactionGateway
    {
        public Task<int> NumberOfRowsExtractedResult { get; private set; }

        /*public DmTransactionEntityGatewayProxy(DatabaseContext context) : base(context,)
        {
        }*/
        public DmTransactionEntityGatewayProxy(DatabaseContext context, IAmazonDynamoDB amazonDynamoDb) :
            base(context, amazonDynamoDb)
        {
        }

        public new Task<int> ExtractAsync()
        {
            NumberOfRowsExtractedResult = base.ExtractAsync();
            return NumberOfRowsExtractedResult;
        }

    }
}
