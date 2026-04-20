using System.Globalization;

namespace FreelanceHub.Web.Extensions;

public static class CultureExtensions
{
    public static string NormalizeSiteCulture(this string? culture) =>
        string.Equals(culture, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "fr";

    public static bool IsFrench(this string? culture) =>
        culture.NormalizeSiteCulture() == "fr";

    public static void ApplyCulture(this HttpContext httpContext, string culture)
    {
        var normalizedCulture = culture.NormalizeSiteCulture();
        var cultureInfo = new CultureInfo(normalizedCulture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }
}
