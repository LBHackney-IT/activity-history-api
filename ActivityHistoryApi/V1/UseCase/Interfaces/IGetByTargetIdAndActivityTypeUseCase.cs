using Hackney.Core.DynamoDb;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Boundary.Response;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.UseCase.Interfaces
{
    public interface IGetByTargetIdAndActivityTypeUseCase
    {
        Task<PagedResult<ActivityHistoryResponseObject>> ExecuteAsync(GetActivityHistoryByTargetIdAndActivityTypeQuery query);
    }
}
