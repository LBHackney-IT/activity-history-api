using System.Threading.Tasks;
using Hackney.Core.DynamoDb;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Domain;

namespace ActivityHistoryApi.V1.Gateways
{
    public interface IActivityHistoryGateway
    {
        Task<PagedResult<ActivityHistoryEntity>> GetByTargetIdAsync(GetActivityHistoryByTargetIdQuery query);
    }
}
