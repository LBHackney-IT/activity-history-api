using FluentAssertions;
using Hackney.Core.DynamoDb;
using Hackney.Shared.ActivityHistory.Boundary.Response;
using Hackney.Shared.ActivityHistory.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActivityHistoryApi.Tests.V1.E2ETests.Steps
{
    public class GetActivityHistorySteps : BaseSteps
    {
        private readonly List<ActivityHistoryResponseObject> _pagedActivityHist = new List<ActivityHistoryResponseObject>();

        public GetActivityHistorySteps(HttpClient httpClient) : base(httpClient)
        { }

        private static bool IsDateTimeListInDescendingOrder(IEnumerable<DateTime> dateTimeList)
        {
            var previousDateTimeItem = dateTimeList.FirstOrDefault();
            foreach (DateTime currentDateTimeItem in dateTimeList)
            {
                if (currentDateTimeItem.CompareTo(previousDateTimeItem) > 0)
                    return false;
            }

            return true;
        }

        private async Task<HttpResponseMessage> CallApi(string id, string paginationToken = null, int? pageSize = null)
        {
            var route = $"api/v1/activityHistory?targetId={id}";
            if (!string.IsNullOrEmpty(paginationToken))
                route += $"&paginationToken={paginationToken}";
            if (pageSize.HasValue)
                route += $"&pageSize={pageSize.Value}";
            var uri = new Uri(route, UriKind.Relative);
            return await _httpClient.GetAsync(uri).ConfigureAwait(false);
        }

        private async Task<PagedResult<ActivityHistoryResponseObject>> ExtractResultFromHttpResponse(HttpResponseMessage response)
        {
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var apiResult = JsonSerializer.Deserialize<PagedResult<ActivityHistoryResponseObject>>(responseContent, _jsonOptions);
            return apiResult;
        }

        public async Task WhenTheActivityHistoryAreRequested(string id)
        {
            _lastResponse = await CallApi(id).ConfigureAwait(false);
        }

        public async Task WhenTheActivityHistoryAreRequestedWithPageSize(string id, int? pageSize = null)
        {
            _lastResponse = await CallApi(id, null, pageSize).ConfigureAwait(false);
        }

        public async Task WhenAllTheActivityHistoryAreRequested(string id)
        {
            string pageToken = null;
            do
            {
                var response = await CallApi(id, pageToken).ConfigureAwait(false);
                var apiResult = await ExtractResultFromHttpResponse(response).ConfigureAwait(false);
                _pagedActivityHist.AddRange(apiResult.Results);

                pageToken = apiResult.PaginationDetails.NextToken;
            }
            while (!string.IsNullOrEmpty(pageToken));
        }

        public async Task ThenTheActivityHistorysAreReturned(List<ActivityHistoryDB> activityHistoryDB)
        {
            var apiResult = await ExtractResultFromHttpResponse(_lastResponse).ConfigureAwait(false);
            apiResult.Results.Should().BeEquivalentTo(activityHistoryDB);
            IsDateTimeListInDescendingOrder(apiResult.Results.Select(x => x.CreatedAt)).Should().BeTrue();
        }

        public async Task ThenAllTheActivityHistoryAreReturnedWithNoPaginationToken(List<ActivityHistoryDB> activityHistoryDB)
        {
            var apiResult = await ExtractResultFromHttpResponse(_lastResponse).ConfigureAwait(false);
            apiResult.Results.Should().BeEquivalentTo(activityHistoryDB);
            IsDateTimeListInDescendingOrder(apiResult.Results.Select(x => x.CreatedAt)).Should().BeTrue();
            apiResult.PaginationDetails.HasNext.Should().BeFalse();
            apiResult.PaginationDetails.NextToken.Should().BeNull();
        }

        public async Task ThenTheActivityHistoryAreReturnedByPageSize(List<ActivityHistoryDB> activityHistoryDB, int? pageSize)
        {
            var expectedPageSize = 10;
            if (pageSize.HasValue)
                expectedPageSize = (pageSize.Value > activityHistoryDB.Count) ? activityHistoryDB.Count : pageSize.Value;

            var apiResult = await ExtractResultFromHttpResponse(_lastResponse).ConfigureAwait(false);
            apiResult.Results.Count.Should().Be(expectedPageSize);
            apiResult.Results.Should().BeEquivalentTo(activityHistoryDB.OrderByDescending(x => x.CreatedAt).Take(expectedPageSize));

            IsDateTimeListInDescendingOrder(apiResult.Results.Select(x => x.CreatedAt)).Should().BeTrue();
        }

        public async Task ThenTheFirstPageOfActivityHistoryAreReturned(List<ActivityHistoryDB> activityHistoryDB)
        {
            var apiResult = await ExtractResultFromHttpResponse(_lastResponse).ConfigureAwait(false);
            apiResult.PaginationDetails.NextToken.Should().NotBeNullOrEmpty();
            apiResult.Results.Count.Should().Be(10);
            apiResult.Results.Should().BeEquivalentTo(activityHistoryDB.OrderByDescending(x => x.CreatedAt).Take(10));

            IsDateTimeListInDescendingOrder(apiResult.Results.Select(x => x.CreatedAt)).Should().BeTrue();
        }

        public void ThenAllTheActivityHistoryAreReturned(List<ActivityHistoryDB> activityHistoryDB)
        {
            _pagedActivityHist.Should().BeEquivalentTo(activityHistoryDB.OrderByDescending(x => x.CreatedAt));
            IsDateTimeListInDescendingOrder(_pagedActivityHist.Select(x => x.CreatedAt)).Should().BeTrue();
        }

        public void ThenBadRequestIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void ThenNotFoundIsReturned()
        {
            _lastResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
