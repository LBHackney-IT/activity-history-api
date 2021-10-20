using Microsoft.AspNetCore.Mvc;
using System;

namespace ActivityHistoryApi.V1.Boundary.Request
{
    public class GetActivityHistoryByTargetIdQuery
    {
        [FromQuery]
        public Guid TargetId { get; set; }

        [FromQuery]
        public string PaginationToken { get; set; }

        [FromQuery]
        public int? PageSize { get; set; }
    }
}
