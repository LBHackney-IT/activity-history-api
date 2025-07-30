using ActivityHistoryApi.V1.Gateways;
using ActivityHistoryApi.V1.UseCase.Interfaces;
using Hackney.Core.DynamoDb;
using Hackney.Core.Logging;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Boundary.Response;
using Hackney.Shared.ActivityHistory.Factories;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.UseCase
{
    public class GetByTargetIdAndActivityTypeUseCase : IGetByTargetIdAndActivityTypeUseCase
    {
        private readonly IActivityHistoryGateway _gateway;
        public GetByTargetIdAndActivityTypeUseCase(IActivityHistoryGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PagedResult<ActivityHistoryResponseObject>> ExecuteAsync(GetActivityHistoryByTargetIdAndActivityTypeQuery query)
        {
            var gatewayResult = await _gateway.GetByTargetIdAndActivityTypeAsync(query).ConfigureAwait(false);
            return new PagedResult<ActivityHistoryResponseObject>(gatewayResult.Results.ToResponse(), gatewayResult.PaginationDetails);
        }
    }
}
