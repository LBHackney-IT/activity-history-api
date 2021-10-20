using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Response;
using Hackney.Core.DynamoDb;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.UseCase.Interfaces
{
    public interface IGetByTargetIdUseCase
    {
        Task<PagedResult<ActivityHistoryResponseObject>> ExecuteAsync(GetActivityHistoryByTargetIdQuery query);
    }
}
