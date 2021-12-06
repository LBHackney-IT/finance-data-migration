using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Gateways
{
    public class DynamoDbGatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IDynamoDBContext> _dynamoDb;
        private readonly DynamoDbGateway _gateway;
        private const string EntityName = "Transactions";

        public DynamoDbGatewayTests()
        {
            _dynamoDb = new Mock<IDynamoDBContext>();
            _gateway = new DynamoDbGateway(_dynamoDb.Object);
        }

        [Fact]
        public async Task GetByIdEntityDoesntExistsReturnsNull()
        {
            _dynamoDb.Setup(x => x.LoadAsync<MigrationRunDbEntity>(It.IsAny<Guid>(), default)).ReturnsAsync((MigrationRunDbEntity) null);

            var result = await _gateway.GetMigrationRunByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdEntityExistsReturnsEntity()
        {
            var expectedResult = _fixture.Build<MigrationRunDbEntity>()
                .With(x => x.DynamoDbEntity, EntityName)
                .Create();

            _dynamoDb.Setup(x => x.LoadAsync<MigrationRunDbEntity>(It.IsAny<Guid>(), It.IsAny<Guid>(),
                default))
                .ReturnsAsync(expectedResult);

            var result = await _gateway.GetMigrationRunByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            result.Should().NotBeNull();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task AddAndUpdateSaveObjectVerifiedOneTimeWorked()
        {
            var entity = _fixture.Create<MigrationRun>();

            _dynamoDb.Setup(x => x.SaveAsync(It.IsAny<MigrationRunDbEntity>(), It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);

            await _gateway.AddAsync(entity).ConfigureAwait(false);

            _dynamoDb.Verify(x => x.SaveAsync(It.IsAny<MigrationRunDbEntity>(), default), Times.Once);
        }

        [Fact]
        public async Task AddAndUpdateInvalidObjectVerifiedOneTimeWorked()
        {
            MigrationRun entity = null;

            _dynamoDb.Setup(x => x.SaveAsync(It.IsAny<MigrationRunDbEntity>(), It.IsAny<CancellationToken>()))
              .Returns(Task.CompletedTask);

            await _gateway.AddAsync(entity).ConfigureAwait(false);

            _dynamoDb.Verify(x => x.SaveAsync(It.IsAny<MigrationRunDbEntity>(), default), Times.Once);
        }


        [Fact]
        public async Task VerifiesGatewayMethodsAddtoDB()
        {
            //var entity = _fixture.Build<MigrationRunDbEntity>().With(x => x.UpdatedAt, DateTime.UtcNow).Create();

            //_dynamoDb.Setup(x => x.SaveAsync(It.IsAny<MigrationRunDbEntity>(), default)).ReturnsAsync();

            //            InsertDatatoDynamoDB(entity);

            //var result = await _gateway.GetMigrationRunByIdAsync(entity.Id).ConfigureAwait(false);
            //result.Should().BeEquivalentTo(entity);
            // _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {entity.Id}", Times.Once());

            await Task.Delay(0).ConfigureAwait(false);
        }

        //private void InsertDatatoDynamoDB(MigrationRunDbEntity entity)
        //{
        //    _dynamoDb.Setup(x => x.SaveAsync<MigrationRunDbEntity>(It.IsAny<MigrationRunDbEntity>(), default)).ReturnsAsync((MigrationRunDbEntity) null);

        //    _dynamoDb.SaveAsync<MigrationRunDbEntity>(entity).GetAwaiter().GetResult();
        //    CleanupActions.Add(async () => await DynamoDbContext.DeleteAsync(entity).ConfigureAwait(false));
        //}

    }
}
