using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FluentAssertions;
using Hackney.Shared.Tenure.Domain;
using Moq;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.UseCase
{
    public class TransformTransactionEntityUseCaseTests
    {
        private Mock<IDMRunLogGateway> _dMRunLogGateway;
        private Mock<IDMTransactionEntityGateway> _dMTransactionEntityGateway;
        private Mock<ITenureGateway> _tenureGateway;
        private Mock<IPersonGateway> _personGateway;
        private static readonly Guid _householdmemberId = Guid.NewGuid();


        public TransformTransactionEntityUseCaseTests()
        {
            //setup defaults as mocks
            _dMRunLogGateway = new Mock<IDMRunLogGateway>();
            _dMTransactionEntityGateway = new Mock<IDMTransactionEntityGateway>();
            _tenureGateway = new Mock<ITenureGateway>();
            _personGateway = new Mock<IPersonGateway>();
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
                _tenureGateway.Object,
                _personGateway.Object);

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
                _tenureGateway.Object,
                _personGateway.Object);

            var transactionPerson = await sut.GetTransactionPersonAsync("123").ConfigureAwait(true);


            //assert
            transactionPerson.Should().BeNullOrEmpty();
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
