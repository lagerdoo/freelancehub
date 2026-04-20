using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Web.Extensions;

public static class LocalizedContentExtensions
{
    public static string Localize(this SiteSetting setting, string culture, Func<SiteSetting, string> english, Func<SiteSetting, string> french) =>
        culture.IsFrench() ? french(setting) : english(setting);

    public static string GetTitle(this Service service, string culture) => culture.IsFrench() ? service.TitleFr : service.TitleEn;
    public static string GetSummary(this Service service, string culture) => culture.IsFrench() ? service.SummaryFr : service.SummaryEn;
    public static string GetDescription(this Service service, string culture) => culture.IsFrench() ? service.DescriptionFr : service.DescriptionEn;

    public static string GetTitle(this Project project, string culture) => culture.IsFrench() ? project.TitleFr : project.TitleEn;
    public static string GetSummary(this Project project, string culture) => culture.IsFrench() ? project.SummaryFr : project.SummaryEn;
    public static string GetDescription(this Project project, string culture) => culture.IsFrench() ? project.DescriptionFr : project.DescriptionEn;

    public static string GetRole(this ExperienceEntry entry, string culture) => culture.IsFrench() ? entry.RoleFr : entry.RoleEn;
    public static string GetDescription(this ExperienceEntry entry, string culture) => culture.IsFrench() ? entry.DescriptionFr : entry.DescriptionEn;
}
