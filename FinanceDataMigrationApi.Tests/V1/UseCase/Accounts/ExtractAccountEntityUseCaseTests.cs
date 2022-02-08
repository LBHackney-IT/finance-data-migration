using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.UseCase.Accounts
{
    public class ExtractAccountEntityUseCaseTests
    {
        /* private readonly Fixture _fixture;
         private Mock<IDMRunLogGateway> _dMRunLogGateway;
         private Mock<IDMAccountEntityGateway> _dMAccountEntityGateway;
         private readonly int _waitDuration = 25;

         private ExtractAccountEntityUseCase _sut;

         public ExtractAccountEntityUseCaseTests()
         {
             _dMRunLogGateway = new Mock<IDMRunLogGateway>();
             _dMAccountEntityGateway = new Mock<IDMAccountEntityGateway>();
             _fixture = new Fixture();

             _sut = new ExtractAccountEntityUseCase(_dMRunLogGateway.Object, _dMAccountEntityGateway.Object);
         }

         [Fact]
         public async Task Extract10RecordsReturnsSuccess()
         {
             int expectedExtractedRowsCount = 10;
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                 .ReturnsAsync(new DMRunLogDomain()
                 {
                     LastRunStatus = "Success",
                     DynamoDbTableName = DMEntityNames.Accounts
                 });
             var logRecord = _fixture.Create<DMRunLogDomain>();
             _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
                 .ReturnsAsync(logRecord);

             _dMAccountEntityGateway.Setup(_ => _.ExtractAsync(It.IsAny<DateTimeOffset?>()))
                 .ReturnsAsync(expectedExtractedRowsCount);

             var actualResult = await _sut.ExecuteAsync().ConfigureAwait(false);

             actualResult.Should().BeEquivalentTo(new StepResponse
             {
                 Continue = true
             }, options => options.Excluding(_ => _.NextStepTime));

             actualResult.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration), 100);

             logRecord.ExpectedRowsToMigrate = expectedExtractedRowsCount;
             logRecord.LastRunStatus = MigrationRunStatus.ExtractCompleted.ToString();

             _dMRunLogGateway.Verify(_ => _.UpdateAsync(logRecord), Times.Once);
         }

         [Fact]
         public async Task ExtractNoRecordsLogError()
         {
             int expectedExtractedRowsCount = 0;
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                 .ReturnsAsync(new DMRunLogDomain()
                 {
                     LastRunStatus = "Success",
                     DynamoDbTableName = DMEntityNames.Accounts
                 });
             var logRecord = _fixture.Create<DMRunLogDomain>();
             _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
                 .ReturnsAsync(logRecord);

             _dMAccountEntityGateway.Setup(_ => _.ExtractAsync(It.IsAny<DateTime>()))
                 .ReturnsAsync(expectedExtractedRowsCount);

             var actualResult = await _sut.ExecuteAsync().ConfigureAwait(false);

             actualResult.Should().BeEquivalentTo(new StepResponse
             {
                 Continue = true
             }, options => options.Excluding(_ => _.NextStepTime));

             actualResult.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(_waitDuration), 100);

             logRecord.ExpectedRowsToMigrate = expectedExtractedRowsCount;
             logRecord.LastRunStatus = MigrationRunStatus.ExtractFailed.ToString();

             _dMRunLogGateway.Verify(_ => _.UpdateAsync(logRecord), Times.Once);
         }

         [Fact]
         public async Task LogGatewayGetNameThrowsShouldRethrow()
         {
             var expectedException = new Exception("Some message");
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                .ThrowsAsync(expectedException);

             Func<Task> action = () => _sut.ExecuteAsync();

             var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
             actualException.Should().BeEquivalentTo(expectedException);
         }

         [Fact]
         public async Task LogGatewayAddThrowsShouldRethrow()
         {
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                 .ReturnsAsync(new DMRunLogDomain()
                 {
                     LastRunStatus = "Success",
                     DynamoDbTableName = DMEntityNames.Accounts
                 });

             var expectedException = new Exception("Some message");
             _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
                .ThrowsAsync(expectedException);

             Func<Task> action = () => _sut.ExecuteAsync();

             var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
             actualException.Should().BeEquivalentTo(expectedException);
         }

         [Fact]
         public async Task AccountGatewayExtractThrowsShouldRethrow()
         {
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                 .ReturnsAsync(new DMRunLogDomain()
                 {
                     LastRunStatus = "Success",
                     DynamoDbTableName = DMEntityNames.Accounts
                 });

             var logRecord = _fixture.Create<DMRunLogDomain>();

             _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
                 .ReturnsAsync(logRecord);

             var expectedException = new Exception("Some message");
             _dMAccountEntityGateway.Setup(_ => _.ExtractAsync(It.IsAny<DateTimeOffset?>()))
                .ThrowsAsync(expectedException);

             Func<Task> action = () => _sut.ExecuteAsync();

             var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
             actualException.Should().BeEquivalentTo(expectedException);
         }

         [Fact]
         public async Task LogGatewayUpdateThrowsShouldRethrow()
         {
             int expectedExtractedRowsCount = 10;
             _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
                 .ReturnsAsync(new DMRunLogDomain()
                 {
                     LastRunStatus = "Success",
                     DynamoDbTableName = DMEntityNames.Accounts
                 });

             var logRecord = _fixture.Create<DMRunLogDomain>();

             _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
                 .ReturnsAsync(logRecord);

             _dMAccountEntityGateway.Setup(_ => _.ExtractAsync(It.IsAny<DateTime>()))
                 .ReturnsAsync(expectedExtractedRowsCount);

             var expectedException = new Exception("Some message");
             _dMRunLogGateway.Setup(_ => _.UpdateAsync(logRecord))
                .ThrowsAsync(expectedException);

             Func<Task> action = () => _sut.ExecuteAsync();

             var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
             actualException.Should().BeEquivalentTo(expectedException);
         }*/
    }
}
