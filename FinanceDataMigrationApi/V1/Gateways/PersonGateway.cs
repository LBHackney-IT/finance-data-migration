using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using Hackney.Shared.Person;

namespace FinanceDataMigrationApi.V1.Gateways
{
    public class PersonGateway : IPersonGateway
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDb;

        public PersonGateway(IDynamoDBContext dbContext, IAmazonDynamoDB dynamoDb)
        {
            _dbContext = dbContext;
            _dynamoDb = dynamoDb;
        }

        public async Task<Person> GetById(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentException(nameof(id).ToString());

            return await Task.Run(() => new Person()).ConfigureAwait(false);
        }
    }
}
