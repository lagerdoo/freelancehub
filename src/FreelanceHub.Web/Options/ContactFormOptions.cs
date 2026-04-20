namespace FreelanceHub.Web.Options;

public sealed class ContactFormOptions
{
    public const string SectionName = "ContactForm";

    public int MinimumSubmitDelaySeconds { get; set; } = 3;
    public int TokenLifetimeHours { get; set; } = 12;
    public int MaxSubmissionsPerWindow { get; set; } = 3;
    public int RateLimitWindowMinutes { get; set; } = 15;
}
