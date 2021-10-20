using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Infrastructure;

namespace ActivityHistoryApi.V1.Factories
{
    public static class EntityFactory
    {
        public static ActivityHistoryEntity ToDomain(this ActivityHistoryDB databaseEntity)
        {

            return new ActivityHistoryEntity
            {
                Id = databaseEntity.Id,
                AuthorDetails = databaseEntity.AuthorDetails,
                NewData = databaseEntity.NewData,
                OldData = databaseEntity.OldData,
                TargetId = databaseEntity.TargetId,
                TargetType = databaseEntity.TargetType,
                TimetoLiveForRecord = databaseEntity.TimetoLiveForRecord,
                Type = databaseEntity.Type,
                CreatedAt = databaseEntity.CreatedAt,
                SourceDomain = databaseEntity.SourceDomain
            };
        }

        public static ActivityHistoryDB ToDatabase(this ActivityHistoryEntity entity)
        {

            return new ActivityHistoryDB
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Type = entity.Type,
                OldData = entity.OldData,
                TimetoLiveForRecord = entity.TimetoLiveForRecord,
                TargetType = entity.TargetType,
                TargetId = entity.TargetId,
                NewData = entity.NewData,
                AuthorDetails = entity.AuthorDetails,
                SourceDomain = entity.SourceDomain
            };
        }
    }
}
