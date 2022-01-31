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
        private readonly Fixture _fixture;
        private Mock<IDMRunLogGateway> _dMRunLogGateway;
        private Mock<IDMAccountEntityGateway> _dMAccountEntityGateway;
        private readonly string _waitDuration = Environment.GetEnvironmentVariable("WAIT_DURATION");

        private ExtractAccountEntityUseCase _sut;

        public ExtractAccountEntityUseCaseTests()
        {
            _dMRunLogGateway = new Mock<IDMRunLogGateway>();
            _dMAccountEntityGateway = new Mock<IDMAccountEntityGateway>();
            _fixture = new Fixture();

            _sut = new ExtractAccountEntityUseCase(_dMRunLogGateway.Object, _dMAccountEntityGateway.Object);
        }

        //[Fact]
        //public async Task Extract10RecordsReturnsSuccess()
        //{
        //    Environment.SetEnvironmentVariable("WAIT_DURATION", "5");

        //    _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Accounts))
        //        .ReturnsAsync(new DMRunLogDomain()
        //        {
        //            LastRunStatus = "Success",
        //            DynamoDbTableName = DMEntityNames.Accounts
        //        });
        //    _dMRunLogGateway.Setup(x => x.AddAsync(It.IsAny<DMRunLogDomain>()))
        //        .ReturnsAsync(_fixture.Create<DMRunLogDomain>());

        //    _dMAccountEntityGateway.Setup(_ => _.ExtractAsync(It.IsAny<DateTime>()))
        //        .ReturnsAsync(10);

        //    var actualResult = await _sut.ExecuteAsync().ConfigureAwait(false);

        //    actualResult.Should().BeEquivalentTo(new StepResponse
        //    {
        //        Continue = true
        //    }, options => options.Excluding(_ => _.NextStepTime));
        //    actualResult.NextStepTime.Should().BeCloseTo(DateTime.Now.AddSeconds(int.Parse(_waitDuration)));
        //}
    }
}
