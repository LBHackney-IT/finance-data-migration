using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Tenure.Infrastructure;
using System;
using System.Threading.Tasks;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class TenureDynamoDbGateway : ITenureDynamoDbGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public TenureDynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task<TenureInformationDb> GetTenureById(Guid tenureId)
        {
            var tenureInformation = await _dynamoDbContext.LoadAsync<TenureInformationDb>(tenureId).ConfigureAwait(false);

            return tenureInformation;
        }
    }
}
