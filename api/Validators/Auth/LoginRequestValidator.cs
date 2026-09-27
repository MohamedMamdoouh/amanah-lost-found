using Amanah.Api.Services.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Auth;
using FluentValidation;

namespace Amanah.Api.Validators.Auth;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
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

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldPasswordRequired);
    }

    private static bool IsChannel(LoginRequest request, AuthIdentifierChannel expected) =>
        AuthIdentifierNormalizer.TryParseChannel(request.Channel, out var parsed)
        && parsed == expected;
}
