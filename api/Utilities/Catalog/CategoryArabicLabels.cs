namespace Amanah.Api.Utilities.Catalog;

public static class CategoryArabicLabels
{
    private static readonly Dictionary<string, string> ByCode = new(StringComparer.Ordinal)
    {
        ["phones"] = "موبايلات",
        ["documents-ids"] = "أوراق رسمية",
        ["wallets"] = "محافظ",
        ["bags"] = "شنط وحقائب",
        ["electronics"] = "أجهزة إلكترونية",
        ["accessories"] = "إكسسوارات",
        ["earpuds"] = "سماعات أذن",
        ["other"] = "أشياء أخرى",
    };

    public static string GetDisplayName(string categoryCode)
    {
        if (string.IsNullOrWhiteSpace(categoryCode))
        {
            return categoryCode;
        }

        return ByCode.TryGetValue(categoryCode, out var label) ? label : categoryCode;
    }
}
