using ActivityHistoryApi.V1.Domain;
using System;
using Type = ActivityHistoryApi.V1.Domain.Type;

namespace ActivityHistoryApi.V1.Boundary.Response
{
    public class ActivityHistoryResponseObject
    {
        public Guid Id { get; set; }

        public Type Type { get; set; }

        public TargetType TargetType { get; set; }

        public Guid TargetId { get; set; }

        public DateTime CreatedAt { get; set; }
        public int TimetoLiveForRecordInDays { get; set; }
        public OldData OldData { get; set; }

        public NewData NewData { get; set; }

        public AuthorDetails AuthorDetails { get; set; }


    }
}
