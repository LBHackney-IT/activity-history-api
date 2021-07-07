using AutoFixture;
using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Factories;
using ActivityHistoryApi.V1.Infrastructure;
using FluentAssertions;
using Xunit;
using Bogus;

namespace ActivityHistoryApi.Tests.V1.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<ActivityHistoryDB>();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Id.Should().Be(entity.Id);
            databaseEntity.CreatedAt.Should().BeSameDateAs(entity.CreatedAt);
            databaseEntity.AuthorDetails.Should().Be(entity.AuthorDetails);
            databaseEntity.NewData.Should().Be(entity.NewData);
            databaseEntity.OldData.Should().Be(entity.OldData);
            databaseEntity.TargetId.Should().Be(entity.TargetId);
            databaseEntity.TargetType.Should().Be(entity.TargetType);
            databaseEntity.TimetoLiveForRecordInDays.Should().Be(entity.TimetoLiveForRecordInDays);
            databaseEntity.Type.Should().Be(entity.Type);

        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<ActivityHistoryEntity>();
            var databaseEntity = entity.ToDatabase();

            entity.Id.Should().Be(databaseEntity.Id);
            entity.CreatedAt.Should().BeSameDateAs(databaseEntity.CreatedAt);
            entity.AuthorDetails.Should().Be(databaseEntity.AuthorDetails);
            entity.NewData.Should().Be(databaseEntity.NewData);
            entity.OldData.Should().Be(databaseEntity.OldData);
            entity.TargetId.Should().Be(databaseEntity.TargetId);
            entity.TargetType.Should().Be(databaseEntity.TargetType);
            entity.TimetoLiveForRecordInDays.Should().Be(databaseEntity.TimetoLiveForRecordInDays);
            entity.Type.Should().Be(databaseEntity.Type);

        }
    }
}
