using Amanah.Api.Services.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Account;
using FluentValidation;

namespace Amanah.Api.Validators.Account;

public sealed class SendLinkIdentifierOtpRequestValidator : AbstractValidator<SendLinkIdentifierOtpRequest>
{
    public SendLinkIdentifierOtpRequestValidator()
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
    }

    private static bool IsChannel(SendLinkIdentifierOtpRequest request, AuthIdentifierChannel expected) =>
        AuthIdentifierNormalizer.TryParseChannel(request.Channel, out var parsed)
        && parsed == expected;
}

public sealed class VerifyLinkIdentifierOtpRequestValidator : AbstractValidator<VerifyLinkIdentifierOtpRequest>
{
    public VerifyLinkIdentifierOtpRequestValidator()
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

        RuleFor(request => request.Code)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldOtpCodeRequired)
            .Must(code => OtpCodeNormalizer.TryNormalize(code, out _))
            .WithMessage(ErrorCodes.FieldOtpCodeInvalid);
    }

    private static bool IsChannel(VerifyLinkIdentifierOtpRequest request, AuthIdentifierChannel expected) =>
        AuthIdentifierNormalizer.TryParseChannel(request.Channel, out var parsed)
        && parsed == expected;
}
