using FreelanceHub.Web.Options;
using FreelanceHub.Web.ViewModels.Public;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FreelanceHub.Web.Security;

public sealed class ContactFormProtector(
    IDataProtectionProvider dataProtectionProvider,
    IMemoryCache memoryCache,
    IOptions<ContactFormOptions> options,
    ILogger<ContactFormProtector> logger)
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("FreelanceHub.ContactForm");
    private readonly ContactFormOptions _options = options.Value;

    public string CreateToken()
    {
        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        return _protector.Protect(issuedAt);
    }

    public bool TryValidate(HttpContext httpContext, ContactFormViewModel form, out string errorCode)
    {
        if (!string.IsNullOrWhiteSpace(form.Website))
        {
            logger.LogWarning("Contact form honeypot was triggered from IP {IpAddress}.", GetClientAddress(httpContext));
            errorCode = "spam";
            return false;
        }

        if (string.IsNullOrWhiteSpace(form.SubmissionToken))
        {
            errorCode = "token";
            return false;
        }

        try
        {
            var rawValue = _protector.Unprotect(form.SubmissionToken);
            if (!long.TryParse(rawValue, out var issuedAtUnix))
            {
                errorCode = "token";
                return false;
            }

            var issuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix);
            var age = DateTimeOffset.UtcNow - issuedAt;

            if (age.TotalSeconds < _options.MinimumSubmitDelaySeconds)
            {
                errorCode = "fast";
                return false;
            }

            if (age.TotalHours > _options.TokenLifetimeHours)
            {
                errorCode = "expired";
                return false;
            }
        }
        catch
        {
            errorCode = "token";
            return false;
        }

        var cacheKey = $"contact-form:{GetClientAddress(httpContext)}";
        var state = memoryCache.Get<ContactSubmissionWindow>(cacheKey);
        if (state is null || state.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            state = new ContactSubmissionWindow
            {
                Attempts = 0,
                ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_options.RateLimitWindowMinutes)
            };
        }

        if (state.Attempts >= _options.MaxSubmissionsPerWindow)
        {
            logger.LogWarning("Contact form rate limit reached from IP {IpAddress}.", GetClientAddress(httpContext));
            errorCode = "rate-limit";
            return false;
        }

        state.Attempts++;
        memoryCache.Set(cacheKey, state, state.ExpiresAtUtc);

        errorCode = string.Empty;
        return true;
    }

    private static string GetClientAddress(HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private sealed class ContactSubmissionWindow
    {
        public int Attempts { get; set; }
        public DateTimeOffset ExpiresAtUtc { get; set; }
    }
}
