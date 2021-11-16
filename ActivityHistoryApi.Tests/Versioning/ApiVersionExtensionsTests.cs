using ActivityHistoryApi.Versioning;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ActivityHistoryApi.Tests.Versioning
{
    public class ApiVersionExtensionsTests
    {
        [Fact]
        public void GetFormattedApiVersionTest()
        {
            var version = new ApiVersion(1, 1);
            version.GetFormattedApiVersion().Should().Be($"v{version.ToString()}");
        }
    }
}
