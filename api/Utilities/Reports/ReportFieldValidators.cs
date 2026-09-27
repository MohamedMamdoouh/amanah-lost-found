using Amanah.Api.Data.Entities;
using Amanah.Api.Utilities.Common;
using Amanah.Contracts.Errors;

namespace Amanah.Api.Utilities.Reports;

internal static class ValidationErrors
{
    public static void Add(
        Dictionary<string, string[]> errors,
        string fieldKey,
        string message)
    {
        if (errors.TryGetValue(fieldKey, out var existing))
        {
            errors[fieldKey] = [.. existing, message];
            return;
        }

        errors[fieldKey] = [message];
    }
}

// Top-level date lost/found validation (Cairo calendar).
public static class ReportDateValidator
{
    // Used as the field key in validation error responses.
    public const string FieldName = "dateLostOrFound";

    public static string? ValidateDateLostOrFound(DateOnly date, DateOnly? todayInCairo = null)
    {
        var today = todayInCairo ?? CairoTime.TodayInCairo();
        var oldestAllowed = today.AddMonths(-12);

        if (date > today)
        {
            return ErrorCodes.FieldReportDateFuture;
        }

        if (date < oldestAllowed)
        {
            return ErrorCodes.FieldReportDateTooOld;
        }

        return null;
    }
}

// Category-specific field validation from DB definitions.
public static class CategoryFieldValidator
{
    public static Dictionary<string, string[]> Validate(
        IEnumerable<CategoryFieldDefinition> definitions,
        IReadOnlyDictionary<string, string> submittedValues)
    {
        var errors = new Dictionary<string, string[]>();
        var definitionByKey = definitions.ToDictionary(definition => definition.FieldKey);

        // Reject keys not defined for this category.
        foreach (var (fieldKey, rawValue) in submittedValues)
        {
            if (!definitionByKey.ContainsKey(fieldKey))
            {
                ValidationErrors.Add(errors, fieldKey, ErrorCodes.FieldCategoryUnknown);
            }
        }

        // Validate required, type, and bounds for each defined field.
        foreach (var definition in definitions)
        {
            submittedValues.TryGetValue(definition.FieldKey, out var rawValue);
            var normalized = TextNormalizer.Normalize(rawValue);

            if (definition.Required && normalized.Length == 0)
            {
                ValidationErrors.Add(errors, definition.FieldKey, ErrorCodes.FieldCategoryRequired);
                continue;
            }

            if (normalized.Length == 0)
            {
                continue;
            }

            ValidateTextField(definition, normalized, errors);
        }

        return errors;
    }

    private static void ValidateTextField(
        CategoryFieldDefinition definition,
        string normalized,
        Dictionary<string, string[]> errors)
    {
        if (definition.MinLength is int minLength && normalized.Length < minLength)
        {
            ValidationErrors.Add(errors, definition.FieldKey, ErrorCodes.FieldCategoryMinLength);
            return;
        }

        if (definition.MaxLength is int maxLength && normalized.Length > maxLength)
        {
            ValidationErrors.Add(errors, definition.FieldKey, ErrorCodes.FieldCategoryMaxLength);
            return;
        }

        if (definition.TextFormat == CategoryTextFormat.LettersAndSpaces
            && !normalized.All(character => char.IsLetter(character) || character == ' '))
        {
            ValidationErrors.Add(errors, definition.FieldKey, ErrorCodes.FieldCategoryLettersAndSpaces);
        }
    }
}

public static class ReportContentValidator
{
    public const string TitleField = "title";
    public const string DescriptionField = "description";
    public const string AreaTextField = "areaText";
    public const string RewardAmountField = "rewardAmount";
    public const string HeldLocationField = "heldLocation";

    private const int TitleMinLength = 5;
    private const int TitleMaxLength = 80;
    private const int DescriptionMinLength = 20;
    private const int DescriptionMaxLength = 1000;
    private const int AreaTextMaxLength = 120;
    private const int RewardMinAmount = 50;
    private const int RewardMaxAmount = 50_000;
    private const int HeldLocationMaxLength = 120;

    public static Dictionary<string, string[]> Validate(
        bool isFoundReport,
        string title,
        string description,
        string? areaText,
        bool hasReward,
        int? rewardAmount,
        string? heldLocation)
    {
        var errors = new Dictionary<string, string[]>();

        ValidateTitle(title, errors);
        ValidateDescription(description, errors);
        ValidateAreaText(areaText, errors);
        ValidateReward(isFoundReport, hasReward, rewardAmount, errors);
        ValidateHeldLocation(isFoundReport, heldLocation, errors);

        return errors;
    }

    private static void ValidateTitle(string title, Dictionary<string, string[]> errors)
    {
        if (title.Length == 0)
        {
            ValidationErrors.Add(errors, TitleField, ErrorCodes.FieldReportTitleRequired);
            return;
        }

        if (title.Length < TitleMinLength)
        {
            ValidationErrors.Add(errors, TitleField, ErrorCodes.FieldReportTitleTooShort);
            return;
        }

        if (title.Length > TitleMaxLength)
        {
            ValidationErrors.Add(errors, TitleField, ErrorCodes.FieldReportTitleTooLong);
        }
    }

    private static void ValidateDescription(string description, Dictionary<string, string[]> errors)
    {
        if (description.Length == 0)
        {
            ValidationErrors.Add(errors, DescriptionField, ErrorCodes.FieldReportDescriptionRequired);
            return;
        }

        if (description.Length < DescriptionMinLength)
        {
            ValidationErrors.Add(errors, DescriptionField, ErrorCodes.FieldReportDescriptionTooShort);
            return;
        }

        if (description.Length > DescriptionMaxLength)
        {
            ValidationErrors.Add(errors, DescriptionField, ErrorCodes.FieldReportDescriptionTooLong);
        }
    }

    private static void ValidateAreaText(string? areaText, Dictionary<string, string[]> errors)
    {
        if (areaText is not null && areaText.Length > AreaTextMaxLength)
        {
            ValidationErrors.Add(errors, AreaTextField, ErrorCodes.FieldReportAreaTooLong);
        }
    }

    private static void ValidateReward(
        bool isFoundReport,
        bool hasReward,
        int? rewardAmount,
        Dictionary<string, string[]> errors)
    {
        if (isFoundReport)
        {
            if (hasReward || rewardAmount is not null)
            {
                ValidationErrors.Add(
                    errors,
                    RewardAmountField,
                    ErrorCodes.FieldReportRewardFoundOnly);
            }

            return;
        }

        if (hasReward)
        {
            if (rewardAmount is not int amount)
            {
                ValidationErrors.Add(errors, RewardAmountField, ErrorCodes.FieldReportRewardRequired);
                return;
            }

            if (amount < RewardMinAmount || amount > RewardMaxAmount)
            {
                ValidationErrors.Add(
                    errors,
                    RewardAmountField,
                    ErrorCodes.FieldReportRewardRange);
            }

            return;
        }

        if (rewardAmount is not null)
        {
            ValidationErrors.Add(errors, RewardAmountField, ErrorCodes.FieldReportRewardEmpty);
        }
    }

    private static void ValidateHeldLocation(
        bool isFoundReport,
        string? heldLocation,
        Dictionary<string, string[]> errors)
    {
        if (!isFoundReport)
        {
            if (!string.IsNullOrEmpty(heldLocation))
            {
                ValidationErrors.Add(errors, HeldLocationField, ErrorCodes.FieldReportHeldLocationLostOnly);
            }

            return;
        }

        if (string.IsNullOrEmpty(heldLocation))
        {
            ValidationErrors.Add(errors, HeldLocationField, ErrorCodes.FieldReportHeldLocationRequired);
            return;
        }

        if (heldLocation.Length > HeldLocationMaxLength)
        {
            ValidationErrors.Add(errors, HeldLocationField, ErrorCodes.FieldReportHeldLocationTooLong);
        }
    }
}

public static class ContactInfoValidator
{
    public static Dictionary<string, string[]> ScanPublicFields(
        string title,
        string description,
        string? areaText,
        string? heldLocation,
        IReadOnlyDictionary<string, string> categoryFieldValues)
    {
        var errors = new Dictionary<string, string[]>();

        ScanField(errors, ReportContentValidator.TitleField, title);
        ScanField(errors, ReportContentValidator.DescriptionField, description);
        ScanField(errors, ReportContentValidator.AreaTextField, areaText);
        ScanField(errors, ReportContentValidator.HeldLocationField, heldLocation);

        foreach (var (fieldKey, value) in categoryFieldValues)
        {
            ScanField(errors, fieldKey, value);
        }

        return errors;
    }

    private static void ScanField(
        Dictionary<string, string[]> errors,
        string fieldKey,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!ContactInfoDetector.ContainsContactInfo(value))
        {
            return;
        }

        errors[fieldKey] = [ErrorCodes.ReportContactInfo];
    }
}
