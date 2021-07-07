using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.Controllers;
using ActivityHistoryApi.V1.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.DynamoDb;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class ActivityHistoryControllerTests
    {
        private readonly Mock<IGetByTargetIdUseCase> _mockgetByTargetIdUseCase;
        private readonly ActivityHistoryApiController _classUnderTest;
        private readonly Fixture _fixture = new Fixture();


        public ActivityHistoryControllerTests()
        {
            _mockgetByTargetIdUseCase = new Mock<IGetByTargetIdUseCase>();
            _classUnderTest = new ActivityHistoryApiController(_mockgetByTargetIdUseCase.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetActivityHistoryByTargetIdAsyncNotFoundReturnsNotFound(string paginationToken)
        {
            // Arrange
            var targetId = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = targetId, PaginationToken = paginationToken };
            _mockgetByTargetIdUseCase.Setup(x => x.ExecuteAsync(query)).ReturnsAsync((PagedResult<ActivityHistoryResponseObject>) null);

            // Act
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(targetId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetActivityHistoryByTargetIdAsyncFoundReturnsResponse(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var activityHistories = _fixture.CreateMany<ActivityHistoryResponseObject>(5).ToList();
            var pagedResult = new PagedResult<ActivityHistoryResponseObject>(activityHistories, new PaginationDetails(paginationToken));
            _mockgetByTargetIdUseCase.Setup(x => x.ExecuteAsync(query)).ReturnsAsync(pagedResult);

            // Act
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(pagedResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public void GetPersonByIdAsyncExceptionIsThrown(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var exception = new ApplicationException("Test exception");
            _mockgetByTargetIdUseCase.Setup(x => x.ExecuteAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

    }
}
