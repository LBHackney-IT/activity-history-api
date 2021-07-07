using Amazon.DynamoDBv2.DataModel;
using ActivityHistoryApi.V1.Gateways;
using FluentAssertions;
using Moq;
using Xunit;
using System;
using AutoFixture;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ActivityHistoryApi.V1.Infrastructure;
using ActivityHistoryApi.V1.Domain;
using System.Threading.Tasks;
using ActivityHistoryApi.V1.Boundary.Request;
using System.Linq;

namespace ActivityHistoryApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class ActivityHistoryGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<ActivityHistoryDbGateway>> _logger;
        private readonly IDynamoDBContext _dynamoDb;
        private readonly ActivityHistoryDbGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();

        public ActivityHistoryGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _logger = new Mock<ILogger<ActivityHistoryDbGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _classUnderTest = new ActivityHistoryDbGateway(_dynamoDb, _logger.Object);
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
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        private List<ActivityHistoryDB> InsertActivityHistory(Guid targetId, int count)
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
                _dynamoDb.SaveAsync(activityHistory).GetAwaiter().GetResult();
                _cleanup.Add(async () => await _dynamoDb.DeleteAsync(activityHistory, default).ConfigureAwait(false));
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

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistory index for targetId {query.TargetId}", Times.Once());
        }

        [Fact]
        public async Task GetByTargetIdReturnsRecords()
        {
            var targetId = Guid.NewGuid();
            var expected = InsertActivityHistory(targetId, 5);

            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = targetId };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expected);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistory index for targetId {query.TargetId}", Times.Once());
        }

        [Fact]
        public async Task GetByTargetIdReturnsRecordsAllPages()
        {
            var targetId = Guid.NewGuid();
            var expected = InsertActivityHistory(targetId, 9);
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

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistory index for targetId {query.TargetId}", Times.Exactly(2));
        }

        [Fact]
        public async Task GetByTargetIdReturnsNoPaginationTokenIfPageSizeEqualsRecordCount()
        {
            var targetId = Guid.NewGuid();
            var expected = InsertActivityHistory(targetId, 10);

            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = targetId, PageSize = 10 };
            var response = await _classUnderTest.GetByTargetIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Results.Should().BeEquivalentTo(expected);
            response.PaginationDetails.HasNext.Should().BeFalse();
            response.PaginationDetails.NextToken.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Querying ActivityHistory index for targetId {query.TargetId}", Times.Once());
        }
    }






}
