using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.UseCase;
using FluentAssertions;
using Hackney.Shared.Tenure.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.UseCase.Transactions
{
    public class TransformTransactionEntityUseCaseTests
    {
        private readonly Fixture _fixture;
        private Mock<IDMRunLogGateway> _dMRunLogGateway;
        private Mock<IDMTransactionEntityGateway> _dMTransactionEntityGateway;
        private Mock<ITenureGateway> _tenureGateway;
        private static readonly Guid _householdmemberId = Guid.NewGuid();
        private readonly int _waitDuration = 25;

        private readonly TransformTransactionEntityUseCase _sut;

        public TransformTransactionEntityUseCaseTests()
        {
            //setup defaults as mocks
            _dMRunLogGateway = new Mock<IDMRunLogGateway>();
            _dMTransactionEntityGateway = new Mock<IDMTransactionEntityGateway>();
            _tenureGateway = new Mock<ITenureGateway>();
            _fixture = new Fixture();

            _sut = new TransformTransactionEntityUseCase(_dMRunLogGateway.Object, _dMTransactionEntityGateway.Object, _tenureGateway.Object);
        }

        [Fact]
        public async void GetTransactionPersonShouldReturnFullNameGivenHouseholdMembers()
        {
            var expected = "\"Joe Blogs\"";

            //mocks
            var tenureList = MockTenureInformation();
            _tenureGateway.Setup(x => x.GetByPrnAsync(It.IsAny<string>()))
                .ReturnsAsync(tenureList);

            var sut = new TransformTransactionEntityUseCase(
                _dMRunLogGateway.Object,
                _dMTransactionEntityGateway.Object,
                _tenureGateway.Object);

            var response = await sut.GetTransactionPersonAsync("123").ConfigureAwait(true);

            //assert
            response.Should().Contain(expected);
        }

        [Fact]
        public async void GetTransactionPersonShouldReturnNullGivenTenureInformationEmpty()
        {
            //mocks
            var tenureList = MockTenureInformation();
            _tenureGateway.Setup(x => x.GetByPrnAsync(It.IsAny<string>()))
                .ReturnsAsync((List<TenureInformation>) null);

            var sut = new TransformTransactionEntityUseCase(
                _dMRunLogGateway.Object,
                _dMTransactionEntityGateway.Object,
                _tenureGateway.Object);

            var transactionPerson = await sut.GetTransactionPersonAsync("123").ConfigureAwait(true);

            //assert
            transactionPerson.Should().BeNull();
        }

        [Fact]
        public async Task LogGatewayThrowsShouldRethrow()
        {
            var expectedException = new Exception("Some message");
            _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions))
               .ThrowsAsync(expectedException);

            Func<Task> action = () => _sut.ExecuteAsync();

            var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task NoRowsToTransformReturnsResponse()
        {
            _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions))
               .ReturnsAsync(_fixture
                                .Build<DMRunLogDomain>()
                                .With(x => x.ExpectedRowsToMigrate, 0)
                                .Create<DMRunLogDomain>());
            var expectedResult = new StepResponse()
            {
                Continue = true,
                NextStepTime = DateTime.Now.AddSeconds(_waitDuration)
            };

            var actualResult = await _sut.ExecuteAsync().ConfigureAwait(false);

            actualResult.Should().NotBeNull();
            actualResult.Should().BeEquivalentTo(expectedResult, o => o.Excluding(_ => _.NextStepTime));
            actualResult.NextStepTime.Should().BeCloseTo(expectedResult.NextStepTime, 100);
        }

        [Fact]
        public async Task ThansactionsGatewayThrowsShoulRethrow()
        {
            _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions))
               .ReturnsAsync(_fixture.Create<DMRunLogDomain>());

            var expectedException = new Exception("Some message");
            _dMTransactionEntityGateway.Setup(_ => _.ListAsync())
               .ThrowsAsync(expectedException);

            Func<Task> action = () => _sut.ExecuteAsync();

            var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
            actualException.Should().BeEquivalentTo(expectedException);
        }

        [Fact]
        public async Task TenureGatewayThrowsShouldRethrow()
        {
            _dMRunLogGateway.Setup(_ => _.GetDMRunLogByEntityNameAsync(DMEntityNames.Transactions))
              .ReturnsAsync(_fixture.Create<DMRunLogDomain>());

            _dMTransactionEntityGateway.Setup(x => x.ListAsync())
                .ReturnsAsync(_fixture.Create<List<DMTransactionEntityDomain>>());

            var expectedException = new Exception("Some message");
            _tenureGateway.Setup(_ => _.GetByPrnAsync(It.IsAny<string>()))
               .ThrowsAsync(expectedException);

            Func<Task> action = () => _sut.ExecuteAsync();

            var actualException = await Assert.ThrowsAsync<Exception>(action).ConfigureAwait(false);
            actualException.Should().BeEquivalentTo(expectedException);
        }

        #region MockHelpers

        private static List<TenureInformation> MockTenureInformation()
        {
            //arrange
            var tenureList = new List<TenureInformation>()
            {
                new TenureInformation()
                {
                    Id = _householdmemberId,
                    HouseholdMembers = new List<HouseholdMembers>()
                    {
                        new HouseholdMembers()
                        {
                            FullName = "Joe Blogs",
                            IsResponsible = true
                        }
                    }
                }
            };
            return tenureList;
        }

        #endregion
    }
}
