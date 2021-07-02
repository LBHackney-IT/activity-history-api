using System.Collections.Generic;
using System.Linq;
using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.Domain;

namespace ActivityHistoryApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static ActivityHistoryResponseObject ToResponse(this ActivityHistoryEntity domain)
        {
            return new ActivityHistoryResponseObject
            {
                Id = domain.Id,
                TargetId = domain.TargetId,
                AuthorDetails = domain.AuthorDetails,
                NewData = domain.NewData,
                TargetType = domain.TargetType,
                TimetoLiveForRecordInDays = domain.TimetoLiveForRecordInDays,
                OldData = domain.OldData,
                Type = domain.Type,
                CreatedAt = domain.CreatedAt
            };
        }

        public static List<ActivityHistoryResponseObject> ToResponse(this IEnumerable<ActivityHistoryEntity> domainList)
        {
            if (null == domainList) return new List<ActivityHistoryResponseObject>();

            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
