using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using FinanceDataMigrationApi.V1.Domain.Accounts;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Gateways
{
    public class AccountsDynamoDbGatewayTests
    {
        private readonly Fixture _fixture;
        private readonly AccountsDynamoDbGateway _sut;
        private readonly Mock<IAmazonDynamoDB> _mockAmazonDynamoDB;
        private readonly Mock<ILogger<AccountsDynamoDbGateway>> _mockLogger;


        public AccountsDynamoDbGatewayTests()
        {
            _mockAmazonDynamoDB = new Mock<IAmazonDynamoDB>();
            _mockLogger = new Mock<ILogger<AccountsDynamoDbGateway>>();

            _fixture = new Fixture();
            _fixture.Customize<DMAccountEntity>(c =>
                c.With(a => a.Tenure, JsonConvert.SerializeObject(_fixture.Create<TenureDbEntity>()))
                .With(a => a.ConsolidatedCharges, JsonConvert.SerializeObject(_fixture.Create<List<ConsolidatedChargeDbEntity>>())));


            _sut = new AccountsDynamoDbGateway(_mockAmazonDynamoDB.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task BatchInsertNullAccountsShouldRetunrTrueAndLog()
        {
            List<DMAccountEntity> accounts = null;

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertEmptyAccountsShouldRetunrTrueAndLog()
        {
            List<DMAccountEntity> accounts = new List<DMAccountEntity>(0);

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertPopulatedAccountsShouldReturnTrue()
        {
            List<DMAccountEntity> accounts = _fixture.Create<List<DMAccountEntity>>();

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertRepoThrowNotFoundExceptionShouldLogError()
        {
            List<DMAccountEntity> accounts = _fixture.Create<List<DMAccountEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new ResourceNotFoundException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }

        [Fact]
        public async Task BatchInsertRepoThrowIntrernalExceptionShouldLogError()
        {
            List<DMAccountEntity> accounts = _fixture.Create<List<DMAccountEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new InternalServerErrorException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }

        [Fact]
        public async Task BatchInsertRepoThrowCanceledExceptionShouldLogError()
        {
            List<DMAccountEntity> accounts = _fixture.Create<List<DMAccountEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new TransactionCanceledException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }
    }
}
