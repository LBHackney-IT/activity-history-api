using System.Collections.Generic;
using ActivityHistoryApi.V1.Controllers;
using ActivityHistoryApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Controllers
{
    public class BaseControllerTests
    {
        private BaseController _classUnderTest;
        private ControllerContext _controllerContext;
        private HttpContext _stubHttpContext;

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
            _stubHttpContext.Request.Headers.Add(Constants.CorrelationId, "123");

            // Act
            var result = _classUnderTest.GetCorrelationId();

            // Assert
            result.Should().BeEquivalentTo("123");
        }
    }
}
