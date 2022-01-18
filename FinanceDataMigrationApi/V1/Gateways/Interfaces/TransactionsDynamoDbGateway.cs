using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using FinanceDataMigrationApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways.Interfaces
{
    public class TransactionsDynamoDbGateway : ITransactionsDynamoDbGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public TransactionsDynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task<List<TransactionDbEntity>> GetTenureByTenureId(Guid tenureId)
        {
            var transactions = await _dynamoDbContext
                .QueryAsync<TransactionDbEntity>(tenureId)
                .GetRemainingAsync()
                .ConfigureAwait(false);

            return transactions;
        }
    }
}
