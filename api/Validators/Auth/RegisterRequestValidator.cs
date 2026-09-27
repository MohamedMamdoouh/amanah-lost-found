using Amanah.Api.Utilities.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Auth;
using FluentValidation;

namespace Amanah.Api.Validators.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.SignupToken)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldSignupTokenRequired);

        RuleFor(request => request.DisplayName)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldDisplayNameRequired)
            .Must(DisplayNameValidator.IsValid)
            .WithMessage(ErrorCodes.FieldDisplayNameInvalid);

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldPasswordRequired)
            .MinimumLength(PasswordRules.MinLength)
            .WithMessage(ErrorCodes.FieldPasswordTooShort);

        RuleFor(request => request.AcceptTerms)
            .Equal(true)
            .WithMessage(ErrorCodes.FieldAcceptTermsRequired);
    }
}
