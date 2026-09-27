using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Reports;
using FluentValidation;

namespace Amanah.Api.Validators.Reports;

public sealed class UpdateReportRequestValidator : AbstractValidator<UpdateReportRequest>
{
    public UpdateReportRequestValidator()
    {
        RuleFor(request => request.CategoryCode)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReportCategoryRequired);

        RuleFor(request => request.GovernorateCode)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReportGovernorateRequired);
    }
}
