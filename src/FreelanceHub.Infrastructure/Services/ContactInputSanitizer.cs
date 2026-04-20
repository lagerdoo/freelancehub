using System.Text;

namespace FreelanceHub.Infrastructure.Services;

internal static class ContactInputSanitizer
{
    public static string SanitizeSingleLine(string? value, int maxLength)
    {
        var normalized = Normalize(value, allowLineBreaks: false);
        return normalized.Length > maxLength ? normalized[..maxLength] : normalized;
    }

    public static string SanitizeMultiLine(string? value, int maxLength)
    {
        var normalized = Normalize(value, allowLineBreaks: true);
        return normalized.Length > maxLength ? normalized[..maxLength] : normalized;
    }

    private static string Normalize(string? value, bool allowLineBreaks)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        var previousWasWhitespace = false;

        foreach (var character in value.Trim())
        {
            if (character == '\r')
            {
                continue;
            }

            if (character == '\n' && allowLineBreaks)
            {
                builder.Append('\n');
                previousWasWhitespace = false;
                continue;
            }

            if (char.IsControl(character))
            {
                continue;
            }

            if (char.IsWhiteSpace(character))
            {
                if (!previousWasWhitespace)
                {
                    builder.Append(' ');
                    previousWasWhitespace = true;
                }

                continue;
            }

            builder.Append(character);
            previousWasWhitespace = false;
        }

        return builder.ToString().Trim();
    }
}
