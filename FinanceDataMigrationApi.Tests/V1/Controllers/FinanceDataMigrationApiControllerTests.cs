using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Controllers;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
//using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Controllers
{
//    public class FinanceDataMigrationApiControllerTests : LogCallAspectFixture
    public class FinanceDataMigrationApiControllerTests 
    {
        private readonly FinanceDataMigrationApiController _controller;
        private readonly Mock<IGetMigrationRunByIdUseCase> _mockGetMigrationRunByIdUseCase;
        private readonly Mock<IGetAllUseCase> _mockGetByAllUseCase;
        private readonly Mock<IGetMigrationRunByEntityNameUseCase> _mockGetMigrationRunByEntityNameUseCase;
        private readonly Mock<IUpdateUseCase> _mockUpdateUseCase;
        private readonly Fixture _fixture = new Fixture();

        public FinanceDataMigrationApiControllerTests()
        {
            _mockGetMigrationRunByIdUseCase = new Mock<IGetMigrationRunByIdUseCase>();
            _mockGetByAllUseCase = new Mock<IGetAllUseCase>();
            _mockGetMigrationRunByEntityNameUseCase = new Mock<IGetMigrationRunByEntityNameUseCase>();
            _mockUpdateUseCase = new Mock<IUpdateUseCase>();

            _controller = new FinanceDataMigrationApiController(
                _mockGetMigrationRunByIdUseCase.Object,
                _mockGetByAllUseCase.Object,
                _mockGetMigrationRunByEntityNameUseCase.Object,
                _mockUpdateUseCase.Object);
        }


        [Fact (Skip ="To be fixed!")]
        public async Task GetByIdUseCaseReturnMigrationRunByValidIdShouldReturns200()
        {
            var migrationRunResponse = _fixture.Create<MigrationRunResponse>();
            _mockGetMigrationRunByIdUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Guid>())).ReturnsAsync(migrationRunResponse);

            var result = await _controller.GetById(migrationRunResponse.Id).ConfigureAwait(false);

            result.Should().NotBeNull();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();

            var migrationRun = okResult.Value as MigrationRunResponse;
            migrationRun.Should().NotBeNull();
            migrationRun.Should().BeEquivalentTo(migrationRunResponse);
        }

    }
}
