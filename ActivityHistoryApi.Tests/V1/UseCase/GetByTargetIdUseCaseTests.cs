using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Factories;
using ActivityHistoryApi.V1.Gateways;
using ActivityHistoryApi.V1.UseCase;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.DynamoDb;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByTargetIdUseCaseTests
    {
        private readonly Mock<IActivityHistoryGateway> _mockGateway;
        private readonly GetByTargetIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public GetByTargetIdUseCaseTests()
        {
            _mockGateway = new Mock<IActivityHistoryGateway>();
            _classUnderTest = new GetByTargetIdUseCase(_mockGateway.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetByTargetIdUseCaseGatewayReturnsNullReturnsEmptyList(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var gatewayResult = new PagedResult<ActivityHistoryEntity>(null, new PaginationDetails(paginationToken));
            _mockGateway.Setup(x => x.GetByTargetIdAsync(query)).ReturnsAsync(gatewayResult);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Results.Should().BeEmpty();
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetByTargetIdUseCaseGatewayReturnsEmptyReturnsEmptyList(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var gatewayResult = new PagedResult<ActivityHistoryEntity>(new List<ActivityHistoryEntity>());
            _mockGateway.Setup(x => x.GetByTargetIdAsync(query)).ReturnsAsync(gatewayResult);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public async Task GetByTargetIdUseCaseGatewayReturnsListReturnsResponseList(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var activityHistories = _fixture.CreateMany<ActivityHistoryEntity>(5).ToList();
            var gatewayResult = new PagedResult<ActivityHistoryEntity>(activityHistories, new PaginationDetails(paginationToken));
            _mockGateway.Setup(x => x.GetByTargetIdAsync(query)).ReturnsAsync(gatewayResult);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Results.Should().BeEquivalentTo(activityHistories.ToResponse());
            if (string.IsNullOrEmpty(paginationToken))
                response.PaginationDetails.NextToken.Should().BeNull();
            else
                response.PaginationDetails.DecodeNextToken().Should().Be(paginationToken);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("some-value")]
        public void GetByTargetIdExceptionIsThrown(string paginationToken)
        {
            // Arrange
            var id = Guid.NewGuid();
            var query = new GetActivityHistoryByTargetIdQuery { TargetId = id, PaginationToken = paginationToken };
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetByTargetIdAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<PagedResult<ActivityHistoryResponseObject>>> func = async () =>
                await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }


    }
}
