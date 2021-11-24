using ActivityHistoryApi.Tests.V1.E2ETests.Fixtures;
using ActivityHistoryApi.Tests.V1.E2ETests.Steps;
using Hackney.Core.Testing.DynamoDb;
using System;
using TestStack.BDDfy;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.E2ETests.Stories
{
    [Story(
        AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
        IWant = "to view any changes made by any target entity",
        SoThat = "I have a log of when changes were made and by whom")]
    [Collection("AppTest collection")]
    public class GetActivityHistoryByTargetIdTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ActivityHistoryFixture _activityHistoryFixture;
        private readonly GetActivityHistorySteps _steps;

        public GetActivityHistoryByTargetIdTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _activityHistoryFixture = new ActivityHistoryFixture(_dbFixture.DynamoDbContext);
            _steps = new GetActivityHistorySteps(appFactory.Client);
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
                if (null != _activityHistoryFixture)
                    _activityHistoryFixture.Dispose();

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturnsTheRequestedActivityHistory()
        {
            this.Given(g => _activityHistoryFixture.GivenActivityHistoryAlreadyExist())
                .When(w => _steps.WhenTheActivityHistoryAreRequested(_activityHistoryFixture.TargetId.ToString()))
                .Then(t => _steps.ThenTheActivityHistorysAreReturned(_activityHistoryFixture.ActivityHistories))
                .BDDfy();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(100)]
        public void ServiceReturnsTheRequestedActivityHistoriesByPageSize(int? pageSize)
        {
            this.Given(g => _activityHistoryFixture.GivenActivityHistoryAlreadyExist(30))
                .When(w => _steps.WhenTheActivityHistoryAreRequestedWithPageSize(_activityHistoryFixture.TargetId.ToString(), pageSize))
                .Then(t => _steps.ThenTheActivityHistoryAreReturnedByPageSize(_activityHistoryFixture.ActivityHistories, pageSize))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsFirstPageOfRequestedActivityHistoriesWithPaginationToken()
        {
            this.Given(g => _activityHistoryFixture.GivenActivityHistoryWithMultiplePagesAlreadyExist())
                .When(w => _steps.WhenTheActivityHistoryAreRequested(_activityHistoryFixture.TargetId.ToString()))
                .Then(t => _steps.ThenTheFirstPageOfActivityHistoryAreReturned(_activityHistoryFixture.ActivityHistories))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNoPaginationTokenIfNoMoreResults()
        {
            this.Given(g => _activityHistoryFixture.GivenActivityHistoryAlreadyExist(10))
                .When(w => _steps.WhenTheActivityHistoryAreRequested(_activityHistoryFixture.TargetId.ToString()))
                .Then(t => _steps.ThenAllTheActivityHistoryAreReturnedWithNoPaginationToken(_activityHistoryFixture.ActivityHistories))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsAllPagesActivityHistoriesUsingPaginationToken()
        {
            this.Given(g => _activityHistoryFixture.GivenActivityHistoryWithMultiplePagesAlreadyExist())
                .When(w => _steps.WhenAllTheActivityHistoryAreRequested(_activityHistoryFixture.TargetId.ToString()))
                .Then(t => _steps.ThenAllTheActivityHistoryAreReturned(_activityHistoryFixture.ActivityHistories))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundIfNoActivityHistoryExist()
        {
            this.Given(g => _activityHistoryFixture.GivenATargetIdHasNoActivities())
                .When(w => _steps.WhenTheActivityHistoryAreRequested(_activityHistoryFixture.TargetId.ToString()))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestIfIdInvalid()
        {
            this.Given(g => _activityHistoryFixture.GivenAnInvalidTargetId())
                .When(w => _steps.WhenTheActivityHistoryAreRequested(_activityHistoryFixture.InvalidTargetId))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

    }
}
