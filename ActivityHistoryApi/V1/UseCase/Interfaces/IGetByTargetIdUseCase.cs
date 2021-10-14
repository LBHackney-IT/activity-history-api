using Hackney.Core.DynamoDb;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Boundary.Response;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.UseCase.Interfaces
{
    public interface IGetByTargetIdUseCase
    {
        Task<PagedResult<ActivityHistoryResponseObject>> ExecuteAsync(GetActivityHistoryByTargetIdQuery query);
    }
}
