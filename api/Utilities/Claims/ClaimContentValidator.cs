using Amanah.Api.Utilities.Common;
using Amanah.Api.Utilities.Reports;
using Amanah.Contracts.Errors;

namespace Amanah.Api.Utilities.Claims;

public static class ClaimContentValidator
{
    public const string FieldName = "submittedAnswer";

    public const int MinLength = 10;

    public const int MaxLength = 500;

    public static Dictionary<string, string[]>? Validate(string? rawAnswer)
    {
        var normalized = TextNormalizer.Normalize(rawAnswer);
        var messages = new List<string>();

        if (normalized.Length < MinLength)
        {
            messages.Add(ErrorCodes.FieldSubmittedAnswerTooShort);
        }

        if (normalized.Length > MaxLength)
        {
            messages.Add(ErrorCodes.FieldSubmittedAnswerTooLong);
        }

        if (ContactInfoDetector.ContainsContactInfo(normalized))
        {
            messages.Add(ErrorCodes.ClaimContactInfo);
        }

        if (messages.Count == 0)
        {
            return null;
        }

        return new Dictionary<string, string[]>
        {
            [FieldName] = messages.ToArray(),
        };
    }
}
