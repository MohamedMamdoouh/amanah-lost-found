using Amanah.Api.Utilities.Auth;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Requests.Auth;
using FluentValidation;

namespace Amanah.Api.Validators.Auth;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(request => request.ResetToken)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldResetTokenRequired);

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage(ErrorCodes.FieldPasswordRequired)
            .MinimumLength(PasswordRules.MinLength)
            .WithMessage(ErrorCodes.FieldPasswordTooShort);
    }
}
