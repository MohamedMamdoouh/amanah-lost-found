using Amanah.Api.Utilities.Abuse;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Abuse;
using FluentValidation;

namespace Amanah.Api.Validators.Abuse;

public sealed class FlagListingRequestValidator : AbstractValidator<FlagListingRequest>
{
    public FlagListingRequestValidator()
    {
        RuleFor(request => request.Reason)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldAbuseReasonRequired)
            .Must(AbuseFlagReasons.All.Contains)
            .WithMessage(ErrorCodes.AbuseInvalidReason);

        RuleFor(request => request.Note)
            .MaximumLength(500)
            .When(request => request.Note is not null)
            .WithMessage(ErrorCodes.FieldAbuseNoteTooLong);
    }
}
