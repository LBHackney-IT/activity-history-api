using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.Boundary.Request.Validation
{
    public class GetActivityHistoryByTargetIdValidator : AbstractValidator<GetActivityHistoryByTargetIdQuery>
    {
        public GetActivityHistoryByTargetIdValidator()
        {
            RuleFor(x => x.TargetId).NotNull()
                                    .NotEqual(Guid.Empty);
            RuleFor(x => x.PageSize).GreaterThan(0)
                                    .When(y => y.PageSize.HasValue);
        }
    }
}
