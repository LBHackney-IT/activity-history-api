using ActivityHistoryApi.V1.Gateways;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Domain;
using Hackney.Shared.ActivityHistory.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Gateways
{
    [Collection("AppTest collection")]
    public class ActivityHistoryGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<ActivityHistoryDbGateway>> _logger;
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ActivityHistoryDbGateway _classUnderTest;

        public ActivityHistoryGatewayTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _logger = new Mock<ILogger<ActivityHistoryDbGateway>>();
            _dbFixture = appFactory.DynamoDbFixture;
            _classUnderTest = new ActivityHistoryDbGateway(_dbFixture.DynamoDbContext, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        private async Task InsertDataIntoDynamoDB(ActivityHistoryDB entity)
        {
            await _dbFixture.SaveEntityAsync(entity).ConfigureAwait(false);
        }

        private async Task<List<ActivityHistoryDB>> InsertActivityHistory(Guid targetId, int count)
        {
            var activityHistories = new List<ActivityHistoryDB>();

            var random = new Random();
            Func<DateTime> funcDT = () => DateTime.UtcNow.AddDays(0 - random.Next(100));
            activityHistories.AddRange(_fixture.Build<ActivityHistoryDB>()
                                   .With(x => x.CreatedAt, funcDT)
                                   .With(x => x.TargetType, TargetType.person)
                                   .With(x => x.TargetId, targetId)
                                   .CreateMany(count));

            foreach (var activityHistory in activityHistories)
            {
                await InsertDataIntoDynamoDB(activityHistory).ConfigureAwait(false);
            }

            return activityHistories;
        }

        [Fact]
        public async Task GetByTargetIdReturnsEmptyIfNoRecords()
        {
            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = Guid.NewGuid() };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEmpty();
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistoryByCreatedAt index for targetId {query.TargetId}", Times.Once());
        }

        [Fact]
        public async Task GetByTargetIdReturnsRecords()
        {
            var targetId = Guid.NewGuid();
            var expected = await InsertActivityHistory(targetId, 5).ConfigureAwait(false);

            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = targetId };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expected);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistoryByCreatedAt index for targetId {query.TargetId}", Times.Once());
        }

        [Fact]
        public async Task GetByTargetIdReturnsRecordsAllPages()
        {
            var targetId = Guid.NewGuid();
            var expected = await InsertActivityHistory(targetId, 9).ConfigureAwait(false);
            var expectedFirstPage = expected.OrderByDescending(x => x.CreatedAt).Take(5);
            var expectedSecondPage = expected.Except(expectedFirstPage).OrderByDescending(x => x.CreatedAt);

            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = targetId, PageSize = 5 };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expectedFirstPage);
            response.PaginationDetails.HasNext.Should().BeTrue();
            response.PaginationDetails.NextToken.Should().NotBeNull();

            query.PaginationToken = response.PaginationDetails.NextToken;
            response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expectedSecondPage);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistoryByCreatedAt index for targetId {query.TargetId}", Times.Exactly(2));
        }

        [Fact]
        public async Task GetByTargetIdReturnsNoPaginationTokenIfPageSizeEqualsRecordCount()
        {
            var targetId = Guid.NewGuid();
            var expected = await InsertActivityHistory(targetId, 10).ConfigureAwait(false);

            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = targetId, PageSize = 10 };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expected);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistoryByCreatedAt index for targetId {query.TargetId}", Times.Once());
        }
    }






}
