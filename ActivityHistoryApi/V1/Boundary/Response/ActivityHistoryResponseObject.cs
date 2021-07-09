using ActivityHistoryApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace ActivityHistoryApi.V1.Boundary.Response
{
    public class ActivityHistoryResponseObject
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public ActivityType Type { get; set; }
        public TargetType TargetType { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TimetoLiveForRecord { get; set; }
        public Dictionary<string, object> OldData { get; set; }
        public Dictionary<string, object> NewData { get; set; }
        public AuthorDetails AuthorDetails { get; set; }


    }
}
