using ActivityHistoryApi.V1.Controllers;
using FluentAssertions;
using Hackney.Core.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Controllers
{
    public class BaseControllerTests
    {
        private readonly BaseController _classUnderTest;
        private readonly ControllerContext _controllerContext;
        private readonly HttpContext _stubHttpContext;

        public BaseControllerTests()
        {
            _stubHttpContext = new DefaultHttpContext();
            _controllerContext = new ControllerContext(new ActionContext(_stubHttpContext, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest = new BaseController();

            _classUnderTest.ControllerContext = _controllerContext;
        }

        [Fact]
        public void GetCorrelationShouldThrowExceptionIfCorrelationHeaderUnavailable()
        {
            // Arrange + Act + Assert
            _classUnderTest.Invoking(x => x.GetCorrelationId())
                .Should().Throw<KeyNotFoundException>()
                .WithMessage("Request is missing a correlationId");
        }

        [Fact]
        public void GetCorrelationShouldReturnCorrelationIdWhenExists()
        {
            // Arrange
            _stubHttpContext.Request.Headers.Append(HeaderConstants.CorrelationId, "123");

            // Act
            var result = _classUnderTest.GetCorrelationId();

            // Assert
            result.Should().BeEquivalentTo("123");
        }

        [Fact]
        public void ConfigureJsonSerializerTest()
        {
            BaseController.ConfigureJsonSerializer();

            JsonConvert.DefaultSettings.Should().NotBeNull();
            var settings = JsonConvert.DefaultSettings();
            settings.Formatting.Should().Be(Formatting.Indented);
            settings.ContractResolver.GetType().Should().Be(typeof(CamelCasePropertyNamesContractResolver));
            settings.DateTimeZoneHandling.Should().Be(DateTimeZoneHandling.Utc);
            settings.DateFormatHandling.Should().Be(DateFormatHandling.IsoDateFormat);
        }
    }
}
