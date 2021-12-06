using AutoFixture;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Xunit;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;

namespace FinanceDataMigrationApi.Tests.V1.UseCase
{
    public class GetMigrationRunByEntityNameUseCaseTests
    {
        private readonly Mock<IMigrationRunDynamoGateway> _mockGateway;
        private readonly GetMigrationRunByEntityNameUseCase _getMigrationRunByEntityNameUseCase;
        private readonly Fixture _fixture = new Fixture();

        private const string EntityName = "Transactions";

        public GetMigrationRunByEntityNameUseCaseTests()
        {
            _mockGateway = new Mock<IMigrationRunDynamoGateway>();
            _getMigrationRunByEntityNameUseCase = new GetMigrationRunByEntityNameUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task GetByEntityNameGatewayReturnMigrationRun()
        {
            
            var migrationRun = _fixture.Build<MigrationRun>()
                .With(x => x.DynamoDbEntity, EntityName)
                .Create();

            _mockGateway.Setup(x => x.GetMigrationRunByEntityNameAsync(EntityName)).ReturnsAsync(migrationRun);

            var response = await _getMigrationRunByEntityNameUseCase.ExecuteAsync(EntityName).ConfigureAwait(false);

            response.Should().BeEquivalentTo(migrationRun.ToResponse());
        }

    }
}
