using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.Domain;
using FinanceDataMigrationApi.V1.Factories;

namespace FinancialTransactionsApi.Tests.V1.UseCase
{
    public class GetMigrationRunByIdUseCaseTests
    {
        private readonly Mock<IMigrationRunDynamoGateway> _mockGateway;
        private readonly GetMigrationRunByIdUseCase _getByIdUseCase;
        private readonly Fixture _fixture = new Fixture();

        public GetMigrationRunByIdUseCaseTests()
        {
            _mockGateway = new Mock<IMigrationRunDynamoGateway>();
            _getByIdUseCase = new GetMigrationRunByIdUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task GetByIdGatewayReturnMigrationRun()
        {
            var id = Guid.NewGuid();

            var migrationRun = _fixture.Create<MigrationRun>();

            _mockGateway.Setup(x => x.GetMigrationRunByIdAsync(id)).ReturnsAsync(migrationRun);
            var response = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            response.Should().BeEquivalentTo(migrationRun.ToResponse());
        }

        [Fact]
        public async Task GetByIdGatewayReturnNullReturnNull()
        {
            var id = Guid.NewGuid();
            _mockGateway.Setup(x => x.GetMigrationRunByIdAsync(id)).ReturnsAsync((MigrationRun) null);

            var response = await _getByIdUseCase.ExecuteAsync(id).ConfigureAwait(false);

            response.Should().BeNull();
        }
    }
}
