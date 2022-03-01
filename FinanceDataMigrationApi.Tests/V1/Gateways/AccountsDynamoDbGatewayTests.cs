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
        /*private readonly Fixture _fixture;
        private readonly AccountsGateway _sut;
        private readonly Mock<IAmazonDynamoDB> _mockAmazonDynamoDB;
        private readonly Mock<ILogger<AccountsGateway>> _mockLogger;


        public AccountsDynamoDbGatewayTests()
        {
            _mockAmazonDynamoDB = new Mock<IAmazonDynamoDB>();
            _mockLogger = new Mock<ILogger<AccountsGateway>>();

            _fixture = new Fixture();
            _fixture.Customize<DmAccountDbEntity>(c =>
                c.With(a => a.Tenure, JsonConvert.SerializeObject(_fixture.Create<TenureDbEntity>()))
                .With(a => a.ConsolidatedCharges, JsonConvert.SerializeObject(_fixture.Create<List<DmConsolidatedCharge>>())));


            _sut = new AccountsGateway(_mockAmazonDynamoDB.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task BatchInsertNullAccountsShouldRetunrTrueAndLog()
        {
            List<DmAccountDbEntity> accounts = null;

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertEmptyAccountsShouldRetunrTrueAndLog()
        {
            List<DmAccountDbEntity> accounts = new List<DmAccountDbEntity>(0);

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertPopulatedAccountsShouldReturnTrue()
        {
            List<DmAccountDbEntity> accounts = _fixture.Create<List<DmAccountDbEntity>>();

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(true);
        }

        [Fact]
        public async Task BatchInsertRepoThrowNotFoundExceptionShouldLogError()
        {
            List<DmAccountDbEntity> accounts = _fixture.Create<List<DmAccountDbEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new ResourceNotFoundException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }

        [Fact]
        public async Task BatchInsertRepoThrowIntrernalExceptionShouldLogError()
        {
            List<DmAccountDbEntity> accounts = _fixture.Create<List<DmAccountDbEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new InternalServerErrorException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }

        [Fact]
        public async Task BatchInsertRepoThrowCanceledExceptionShouldLogError()
        {
            List<DmAccountDbEntity> accounts = _fixture.Create<List<DmAccountDbEntity>>();

            _mockAmazonDynamoDB.Setup(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new TransactionCanceledException("some message"));

            var actualResult = await _sut.BatchInsert(accounts).ConfigureAwait(false);

            _mockAmazonDynamoDB.Verify(_ => _.TransactWriteItemsAsync(It.IsAny<TransactWriteItemsRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            actualResult.Should().Be(false);
        }*/
    }
}
