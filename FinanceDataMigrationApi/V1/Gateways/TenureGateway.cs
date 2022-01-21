using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Tenure.Domain;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureGateway: ITenureGateway
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;

        public TenureGateway(IDynamoDBContext dbContext, IAmazonDynamoDB dynamoDb)
        {
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
        }

        public async Task<List<TenureInformation>> GetByPrnAsync(string prn)
        {
            if (prn == null) throw new ArgumentNullException(nameof(prn));

            return await Task.Run(()=>new List<TenureInformation>(5)).ConfigureAwait(false);
        }
    }
}
