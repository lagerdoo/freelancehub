using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/settings")]
public sealed class SettingsController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var settings = await adminContentService.GetSiteSettingAsync(cancellationToken);
        SetPageTitle("Site settings");

        return View(new SiteSettingFormViewModel
        {
            Id = settings.Id,
            SiteName = settings.SiteName,
            TaglineFr = settings.TaglineFr,
            TaglineEn = settings.TaglineEn,
            HeroTitleFr = settings.HeroTitleFr,
            HeroTitleEn = settings.HeroTitleEn,
            HeroSubtitleFr = settings.HeroSubtitleFr,
            HeroSubtitleEn = settings.HeroSubtitleEn,
            AboutSummaryFr = settings.AboutSummaryFr,
            AboutSummaryEn = settings.AboutSummaryEn,
            SkillsFr = settings.SkillsFr,
            SkillsEn = settings.SkillsEn,
            ContactEmail = settings.ContactEmail,
            Location = settings.Location,
            LinkedInUrl = settings.LinkedInUrl,
            GitHubUrl = settings.GitHubUrl,
            MaltUrl = settings.MaltUrl,
            CalendlyUrl = settings.CalendlyUrl,
            FooterTextFr = settings.FooterTextFr,
            FooterTextEn = settings.FooterTextEn,
            LogoPath = settings.LogoPath,
            FaviconPath = settings.FaviconPath,
            MetaTitleFr = settings.MetaTitleFr,
            MetaTitleEn = settings.MetaTitleEn,
            MetaDescriptionFr = settings.MetaDescriptionFr,
            MetaDescriptionEn = settings.MetaDescriptionEn
        });
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(SiteSettingFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            SetPageTitle("Site settings");
            return View(model);
        }

        await adminContentService.SaveSiteSettingAsync(new SiteSettingEditModel
        {
            Id = model.Id,
            SiteName = model.SiteName,
            TaglineFr = model.TaglineFr,
            TaglineEn = model.TaglineEn,
            HeroTitleFr = model.HeroTitleFr,
            HeroTitleEn = model.HeroTitleEn,
            HeroSubtitleFr = model.HeroSubtitleFr,
            HeroSubtitleEn = model.HeroSubtitleEn,
            AboutSummaryFr = model.AboutSummaryFr,
            AboutSummaryEn = model.AboutSummaryEn,
            SkillsFr = model.SkillsFr,
            SkillsEn = model.SkillsEn,
            ContactEmail = model.ContactEmail,
            Location = model.Location,
            LinkedInUrl = model.LinkedInUrl,
            GitHubUrl = model.GitHubUrl,
            MaltUrl = model.MaltUrl,
            CalendlyUrl = model.CalendlyUrl,
            FooterTextFr = model.FooterTextFr,
            FooterTextEn = model.FooterTextEn,
            LogoPath = model.LogoPath,
            FaviconPath = model.FaviconPath,
            MetaTitleFr = model.MetaTitleFr,
            MetaTitleEn = model.MetaTitleEn,
            MetaDescriptionFr = model.MetaDescriptionFr,
            MetaDescriptionEn = model.MetaDescriptionEn
        }, cancellationToken);

        TempData["AdminSuccess"] = "Settings saved successfully.";
        return RedirectToAction(nameof(Index));
    }
}
