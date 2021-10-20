using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Factories;
using FluentAssertions;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Fact]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var domain = new ActivityHistoryEntity();
            var response = domain.ToResponse();
            response.AuthorDetails.Should().Be(domain.AuthorDetails);
            response.CreatedAt.Should().Be(domain.CreatedAt);
            response.Id.Should().Be(domain.Id);
            response.NewData.Should().BeEquivalentTo(domain.NewData);
            response.OldData.Should().BeEquivalentTo(domain.OldData);
            response.TimetoLiveForRecord.Should().Be(domain.TimetoLiveForRecord);
            response.Type.Should().Be(domain.Type);
            response.SourceDomain.Should().Be(domain.SourceDomain);
        }
    }
}
