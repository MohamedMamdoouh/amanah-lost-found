using Amanah.Api.Utilities.Catalog;

namespace Amanah.Api.Services.External.Email;

internal static class AdminAlertEmailTemplates
{
  public static string BuildSubject(string reportType) =>
      string.Equals(reportType, "found", StringComparison.OrdinalIgnoreCase)
          ? "أمانة: بلاغ «عثر عليه» جديد بانتظار المراجعة"
          : "أمانة: بلاغ «مفقود» جديد بانتظار المراجعة";

  public static string BuildPlainText(string reportType, string categoryCode, string reviewLink)
  {
    var typeLabel = GetTypeLabel(reportType);
    var categoryLabel = CategoryArabicLabels.GetDisplayName(categoryCode);
    return
        $"أمانة — قائمة المراجعة\n\n" +
        $"بلاغ جديد ({typeLabel}) ({categoryLabel}) بانتظار مراجعتك.\n\n" +
        $"المراجعة: {reviewLink}\n\n" +
        "هذا التنبيه لا يتضمن بيانات اتصال المبلغ أو محتوى البلاغ الخاص.";
  }

  public static string BuildHtml(string reportType, string categoryCode, string reviewLink)
  {
    var typeLabel = GetTypeLabel(reportType);
    var categoryLabel = CategoryArabicLabels.GetDisplayName(categoryCode);
    var safeCategory = EmailLayout.Encode(categoryLabel);
    var safeLink = EmailLayout.Encode(reviewLink);

    var body = $"""
            <p style="margin:0 0 20px;font-size:16px;color:{EmailDesignTokens.Muted};text-align:right;">
              وصل بلاغ جديد إلى قائمة المراجعة وهو جاهز للاطلاع.
            </p>
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="margin:0 0 24px;background:{EmailDesignTokens.AccentSoft};border:1px solid {EmailDesignTokens.Border};border-radius:12px;">
              <tr>
                <td style="padding:20px;text-align:right;">
                  <p style="margin:0 0 12px;font-size:15px;color:{EmailDesignTokens.Ink};text-align:right;">
                    <span style="color:{EmailDesignTokens.Muted};font-weight:600;">تفاصيل البلاغ:</span>
                    <strong style="font-weight:700;">{typeLabel}</strong>
                  </p>
                  <p style="margin:0;font-size:15px;color:{EmailDesignTokens.Ink};">
                    <span style="color:{EmailDesignTokens.Muted};">التصنيف:</span>
                    <strong style="font-weight:700;">{safeCategory}</strong>
                  </p>
                </td>
              </tr>
            </table>
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:0;">
              <tr>
                <td style="text-align:right;">
                  <a href="{safeLink}" style="display:inline-block;padding:14px 22px;background:{EmailDesignTokens.Primary};color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;border-radius:12px;border:1px solid {EmailDesignTokens.Primary};">
                    فتح في قائمة المراجعة
                  </a>
                </td>
              </tr>
            </table>
            """;

    return EmailLayout.BuildDocument(
        title: "تنبيه مراجعة — أمانة",
        headline: "بلاغ جديد بانتظار المراجعة",
        bodyHtml: body,
        footerHtml: "يستبعد هذا التنبيه عمدًا أرقام هواتف المبلغين وتفاصيل التحقق المخفية والصور.");
  }

  private static string GetTypeLabel(string reportType) =>
      string.Equals(reportType, "found", StringComparison.OrdinalIgnoreCase)
          ? "عثر عليه"
          : "مفقود";

}
