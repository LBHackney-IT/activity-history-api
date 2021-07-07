using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActivityHistoryApi.Tests.V1.Boundary.Request.Validation
{
    public class GetActivityHistoryByTargetIdValidatorTests
    {
        private readonly GetActivityHistoryByTargetIdValidator _classUnderTest;

        public GetActivityHistoryByTargetIdValidatorTests()
        {
            _classUnderTest = new GetActivityHistoryByTargetIdValidator();
        }

        [Fact]
        public void QueryShouldErrorWithNullTargetId()
        {
            var query = new GetActivityHistoryByTargetIdQuery();
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void QueryShouldErrorWithEmptyTargetId()
        {
            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = Guid.Empty };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void QueryShouldNotErrorWithNoPageSize()
        {
            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = Guid.NewGuid() };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void QueryShouldErrorWithInvalidPageSize(int invalidPageSize)
        {
            var query = new GetActivityHistoryByTargetIdQuery() { TargetId = Guid.NewGuid(), PageSize = invalidPageSize };
            var result = _classUnderTest.TestValidate(query);
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
        }
    }
}
