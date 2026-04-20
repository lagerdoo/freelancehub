using FreelanceHub.Web.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelanceHub.Web.Helpers;

public static class HtmlHelperExtensions
{
    public static IHtmlContent Localize(this IHtmlHelper html, string english, string french)
    {
        var culture = html.ViewContext.RouteData.Values["culture"]?.ToString().NormalizeSiteCulture() ?? "fr";
        return new HtmlString(culture.IsFrench() ? french : english);
    }

    public static string SwitchCulturePath(this HttpRequest request, string targetCulture)
    {
        var normalizedCulture = targetCulture.NormalizeSiteCulture();
        var path = request.Path.Value ?? "/";

        if (path == "/")
        {
            return "/" + normalizedCulture;
        }

        if (path.StartsWith("/fr", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/en", StringComparison.OrdinalIgnoreCase))
        {
            return "/" + normalizedCulture + path[3..];
        }

        return "/" + normalizedCulture + path;
    }
}
