using Amanah.Api.Services.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Auth;
using FluentValidation;

namespace Amanah.Api.Validators.Auth;

public sealed class SendOtpRequestValidator : AbstractValidator<SendOtpRequest>
{
    private static readonly HashSet<string> AllowedPurposes =
    [
        OtpPurposes.Signup,
        OtpPurposes.PasswordReset,
    ];

    public SendOtpRequestValidator()
    {
        RuleFor(request => request.Channel)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldSignInChannelRequired)
            .Must(channel => AuthIdentifierNormalizer.TryParseChannel(channel, out _))
            .WithMessage(ErrorCodes.FieldSignInChannelInvalid);

        When(request => IsChannel(request, AuthIdentifierChannel.Phone), () =>
        {
            RuleFor(request => request.Identifier)
                .NotEmpty()
                .WithMessage(ErrorCodes.FieldPhoneRequired)
                .Must(identifier => PhoneNormalizer.TryNormalize(identifier, out _))
                .WithMessage(ErrorCodes.FieldPhoneInvalid);
        });

        When(request => IsChannel(request, AuthIdentifierChannel.Email), () =>
        {
            RuleFor(request => request.Identifier)
                .NotEmpty()
                .WithMessage(ErrorCodes.FieldEmailRequired)
                .Must(identifier => EmailNormalizer.TryNormalize(identifier, out _))
                .WithMessage(ErrorCodes.FieldEmailInvalid);
        });

        RuleFor(request => request.CaptchaToken)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldCaptchaTokenRequired);

        RuleFor(request => request.Purpose)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldOtpPurposeRequired)
            .Must(AllowedPurposes.Contains)
            .WithMessage(ErrorCodes.FieldOtpPurposeInvalid);
    }

    private static bool IsChannel(SendOtpRequest request, AuthIdentifierChannel expected) =>
        AuthIdentifierNormalizer.TryParseChannel(request.Channel, out var parsed)
        && parsed == expected;
}
