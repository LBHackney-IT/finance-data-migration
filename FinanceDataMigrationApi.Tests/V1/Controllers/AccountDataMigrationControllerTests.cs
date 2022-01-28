using AutoFixture;
using FinanceDataMigrationApi.V1.Boundary.Response;
using FinanceDataMigrationApi.V1.Controllers;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDataMigrationApi.Tests.V1.Controllers
{
    public class AccountDataMigrationControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly AccountDataMigrationController _controller;
        private readonly Mock<IExtractAccountEntityUseCase> _mockExtractAccountEntityUseCase;
        private readonly Mock<ITransformAccountsUseCase> _mockTransformAccountsUseCase;
        private readonly Mock<ILoadAccountsUseCase> _mockLoadAccountsUseCase;
        private readonly Mock<IIndexAccountEntityUseCase> _mockIndexAccountEntityUseCase;

        public AccountDataMigrationControllerTests()
        {
            _mockExtractAccountEntityUseCase = new Mock<IExtractAccountEntityUseCase>();
            _mockTransformAccountsUseCase = new Mock<ITransformAccountsUseCase>();
            _mockLoadAccountsUseCase = new Mock<ILoadAccountsUseCase>();
            _mockIndexAccountEntityUseCase = new Mock<IIndexAccountEntityUseCase>();

            _controller = new AccountDataMigrationController(
                _mockIndexAccountEntityUseCase.Object,
                _mockExtractAccountEntityUseCase.Object,
                _mockTransformAccountsUseCase.Object,
                _mockLoadAccountsUseCase.Object);
        }

        [Fact]
        public async Task ExtractAccountEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockExtractAccountEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.ExtractAccountEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExtractAccountEntityUseCaseShouldReturns404()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            stepResponse.Continue = false;

            _mockExtractAccountEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.ExtractAccountEntity().ConfigureAwait(false);

            result.Should().BeEquivalentTo(new NotFoundObjectResult(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Extract Account Entity Task Failed!!")));
        }

        [Fact]
        public async Task TransformAccountEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockTransformAccountsUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.TransformAccountEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task TransformAccountEntityUseCaseShouldReturns500()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            stepResponse.Continue = false;
            var expectedResult = new ObjectResult(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Transform Account Entity Task Failed!!"))
            {
                StatusCode = 500
            };

            _mockTransformAccountsUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.TransformAccountEntity().ConfigureAwait(false);
            var internalServerErrorResult = result as StatusCodeResult;

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task LoadAccountEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockLoadAccountsUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.LoadAccountEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task LoadAccountEntityUseCaseShouldReturns500()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            stepResponse.Continue = false;
            var expectedResult = new ObjectResult(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Load Account Entity Task Failed!!"))
            {
                StatusCode = 500
            };

            _mockLoadAccountsUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.LoadAccountEntity().ConfigureAwait(false);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task IndexTransactionEntityUseCaseShouldReturns200()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            _mockIndexAccountEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.IndexAccountEntity().ConfigureAwait(false);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task IndexTransactionEntityUseCaseShouldReturns404()
        {
            var stepResponse = _fixture.Create<StepResponse>();
            stepResponse.Continue = false;

            _mockIndexAccountEntityUseCase.Setup(x => x.ExecuteAsync()).ReturnsAsync(stepResponse);

            var result = await _controller.IndexAccountEntity().ConfigureAwait(false);

            result.Should().BeEquivalentTo(new NotFoundObjectResult(new BaseErrorResponse((int) HttpStatusCode.InternalServerError, "Index Account Entity Task Failed!!")));
        }
    }
}
