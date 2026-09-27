using Amanah.Api.Utilities.Reports;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Admin;
using FluentValidation;

namespace Amanah.Api.Validators.Admin;

public sealed class RejectReportRequestValidator : AbstractValidator<RejectReportRequest>
{
    public RejectReportRequestValidator()
    {
        RuleFor(request => request.ReasonCode)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldAdminRejectionReasonRequired)
            .Must(RejectionReasonCodes.All.Contains)
            .WithMessage(ErrorCodes.FieldAdminRejectionReasonInvalid);

        RuleFor(request => request.Note)
            .MaximumLength(500)
            .When(request => request.Note is not null)
            .WithMessage(ErrorCodes.FieldAdminRejectionNoteTooLong);
    }
}
