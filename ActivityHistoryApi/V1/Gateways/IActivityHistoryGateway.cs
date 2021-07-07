using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Domain;
using Hackney.Core.DynamoDb;

namespace ActivityHistoryApi.V1.Gateways
{
    public interface IActivityHistoryGateway
    {
        Task<PagedResult<ActivityHistoryEntity>> GetByTargetIdAsync(GetActivityHistoryByTargetIdQuery query);
    }
}
