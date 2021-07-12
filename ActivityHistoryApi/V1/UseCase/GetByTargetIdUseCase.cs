using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.Factories;
using ActivityHistoryApi.V1.Gateways;
using ActivityHistoryApi.V1.UseCase.Interfaces;
using Hackney.Core.DynamoDb;
using Hackney.Core.Logging;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.UseCase
{
    public class GetByTargetIdUseCase : IGetByTargetIdUseCase
    {
        private readonly IActivityHistoryGateway _gateway;
        public GetByTargetIdUseCase(IActivityHistoryGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PagedResult<ActivityHistoryResponseObject>> ExecuteAsync(GetActivityHistoryByTargetIdQuery query)
        {
            var gatewayResult = await _gateway.GetByTargetIdAsync(query).ConfigureAwait(false);
            return new PagedResult<ActivityHistoryResponseObject>(gatewayResult.Results.ToResponse(), gatewayResult.PaginationDetails);
        }
    }
}
