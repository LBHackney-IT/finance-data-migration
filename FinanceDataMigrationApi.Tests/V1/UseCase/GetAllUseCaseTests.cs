using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        private readonly Mock<IMigrationRunDynamoGateway> _mockGateway;
        private readonly GetAllUseCase _getAllUseCase;
        private readonly Fixture _fixture;

        public GetAllUseCaseTests()
        {
            _mockGateway = new Mock<IMigrationRunDynamoGateway>();
            _getAllUseCase = new GetAllUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetAllGatewayReturnsListReturnsList()
        {
            await Task.Delay(0).ConfigureAwait(false);

            //var migrationRuns = _fixture.CreateMany<MigrationRun>();

            //var response = await _getAllUseCase.ExecuteAsync().ConfigureAwait(false);

            //var expectedResponse = migrationRuns.ToResponse();

            //response.MigrationRunResponses.Results.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
