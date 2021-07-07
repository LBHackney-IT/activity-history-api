using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Factories;
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
            //TODO: check here that all of the fields have been mapped correctly. i.e. response.fieldOne.Should().Be("one")
        }
    }
}
