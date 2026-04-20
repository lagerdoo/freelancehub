using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FreelanceHub.Infrastructure.Services;

internal static partial class SlugGenerator
{
    public static string Generate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Guid.NewGuid().ToString("N")[..10];
        }

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var clean = InvalidCharactersRegex().Replace(builder.ToString().Normalize(NormalizationForm.FormC), "-");
        clean = RepeatedDashRegex().Replace(clean, "-").Trim('-');

        return string.IsNullOrWhiteSpace(clean) ? Guid.NewGuid().ToString("N")[..10] : clean;
    }

    [GeneratedRegex(@"[^a-z0-9]+", RegexOptions.Compiled)]
    private static partial Regex InvalidCharactersRegex();

    [GeneratedRegex(@"-+", RegexOptions.Compiled)]
    private static partial Regex RepeatedDashRegex();
}
