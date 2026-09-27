using Amanah.Api.Services.Auth;
using Amanah.Api.Utilities.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Support;
using FluentValidation;

namespace Amanah.Api.Validators.Support;

public sealed class SubmitSupportMessageRequestValidator : AbstractValidator<SubmitSupportMessageRequest>
{
    public const int MessageMinLength = 10;
    public const int MessageMaxLength = 2000;

    public SubmitSupportMessageRequestValidator()
    {
        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldDisplayNameRequired)
            .Must(DisplayNameValidator.IsValid)
            .WithMessage(ErrorCodes.FieldDisplayNameInvalid);

        RuleFor(request => request.ReplyEmail)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldReplyEmailRequired)
            .Must(email => EmailNormalizer.TryNormalize(email, out _))
            .WithMessage(ErrorCodes.FieldReplyEmailInvalid);

        RuleFor(request => request.Message)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldSupportMessageRequired)
            .MinimumLength(MessageMinLength)
            .WithMessage(ErrorCodes.FieldSupportMessageInvalid)
            .MaximumLength(MessageMaxLength)
            .WithMessage(ErrorCodes.FieldSupportMessageInvalid);

        RuleFor(request => request.CaptchaToken)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldCaptchaTokenRequired);
    }
}
