using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Reports;
using FluentValidation;

namespace Amanah.Api.Validators.Reports;

public sealed class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    private static readonly string[] AllowedTypes = ["lost", "found"];

    public CreateReportRequestValidator()
    {
        RuleFor(request => request.Type)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReportTypeRequired)
            .Must(type => AllowedTypes.Contains(type, StringComparer.Ordinal))
            .WithMessage(ErrorCodes.FieldReportTypeInvalid);

        RuleFor(request => request.CategoryCode)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReportCategoryRequired);

        RuleFor(request => request.GovernorateCode)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReportGovernorateRequired);
    }
}
