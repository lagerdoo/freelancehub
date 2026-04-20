using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/cv-import")]
public sealed class CvImportController(ICvImportService cvImportService, IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        SetPageTitle("CV import");
        return View(new CvImportIndexViewModel
        {
            Imports = await cvImportService.GetImportsAsync(cancellationToken)
        });
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(CvImportUploadViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.File is null)
        {
            SetPageTitle("CV import");
            return View(new CvImportIndexViewModel
            {
                Upload = model,
                Imports = await cvImportService.GetImportsAsync(cancellationToken)
            });
        }

        try
        {
            await using var stream = model.File.OpenReadStream();
            var draft = await cvImportService.CreateDraftAsync(new CvImportCreateRequest
            {
                OriginalFileName = model.File.FileName,
                ContentType = model.File.ContentType,
                FileSize = model.File.Length,
                Content = stream
            }, cancellationToken);

            return RedirectToAction(nameof(Review), new { id = draft.ImportId });
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(nameof(model.File), exception.Message);
            SetPageTitle("CV import");
            return View(new CvImportIndexViewModel
            {
                Upload = model,
                Imports = await cvImportService.GetImportsAsync(cancellationToken)
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Review(Guid id, CancellationToken cancellationToken)
    {
        var draft = await cvImportService.GetDraftAsync(id, cancellationToken);
        if (draft is null)
        {
            return NotFound();
        }

        SetPageTitle("Review CV import");
        return View(MapDraft(draft));
    }

    [HttpPost("{id:guid}/apply")]
    public async Task<IActionResult> Apply(Guid id, CvImportReviewViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.ImportId)
        {
            return BadRequest();
        }

        var draft = await cvImportService.GetDraftAsync(id, cancellationToken);
        if (draft is null)
        {
            return NotFound();
        }

        ValidateSelectedSections(model);
        if (!ModelState.IsValid)
        {
            SetPageTitle("Review CV import");
            return View("Review", model);
        }

        var createdServices = 0;
        var createdProjects = 0;
        var createdExperienceEntries = 0;

        if (model.Profile.Selected)
        {
            var current = await adminContentService.GetSiteSettingAsync(cancellationToken);
            await adminContentService.SaveSiteSettingAsync(new SiteSettingEditModel
            {
                Id = current.Id,
                SiteName = ChooseImported(model.Profile.SiteName, current.SiteName),
                TaglineFr = ChooseImported(model.Profile.TaglineFr, current.TaglineFr),
                TaglineEn = ChooseImported(model.Profile.TaglineEn, current.TaglineEn),
                HeroTitleFr = current.HeroTitleFr,
                HeroTitleEn = current.HeroTitleEn,
                HeroSubtitleFr = current.HeroSubtitleFr,
                HeroSubtitleEn = current.HeroSubtitleEn,
                AboutSummaryFr = ChooseImported(model.Profile.AboutSummaryFr, current.AboutSummaryFr),
                AboutSummaryEn = ChooseImported(model.Profile.AboutSummaryEn, current.AboutSummaryEn),
                SkillsFr = ChooseImported(model.Profile.SkillsFr, current.SkillsFr),
                SkillsEn = ChooseImported(model.Profile.SkillsEn, current.SkillsEn),
                ContactEmail = ChooseImported(model.Profile.ContactEmail, current.ContactEmail),
                Location = ChooseImported(model.Profile.Location, current.Location),
                LinkedInUrl = ChooseImported(model.Profile.LinkedInUrl, current.LinkedInUrl),
                GitHubUrl = ChooseImported(model.Profile.GitHubUrl, current.GitHubUrl),
                MaltUrl = current.MaltUrl,
                CalendlyUrl = current.CalendlyUrl,
                FooterTextFr = current.FooterTextFr,
                FooterTextEn = current.FooterTextEn,
                LogoPath = current.LogoPath,
                FaviconPath = current.FaviconPath,
                MetaTitleFr = current.MetaTitleFr,
                MetaTitleEn = current.MetaTitleEn,
                MetaDescriptionFr = current.MetaDescriptionFr,
                MetaDescriptionEn = current.MetaDescriptionEn
            }, cancellationToken);
        }

        var existingServices = await adminContentService.GetServicesAsync(true, cancellationToken);
        var nextServiceOrder = existingServices.Any() ? existingServices.Max(x => x.DisplayOrder) + 1 : 1;
        foreach (var service in model.Services.Where(x => x.Selected))
        {
            await adminContentService.SaveServiceAsync(new ServiceEditModel
            {
                TitleFr = service.TitleFr,
                TitleEn = service.TitleEn,
                SummaryFr = service.SummaryFr,
                SummaryEn = service.SummaryEn,
                DescriptionFr = service.DescriptionFr,
                DescriptionEn = service.DescriptionEn,
                IconClass = string.IsNullOrWhiteSpace(service.IconClass) ? "bi bi-briefcase" : service.IconClass,
                DisplayOrder = nextServiceOrder++,
                IsPublished = false
            }, cancellationToken);
            createdServices++;
        }

        var existingProjects = await adminContentService.GetProjectsAsync(true, cancellationToken);
        var nextProjectOrder = existingProjects.Any() ? existingProjects.Max(x => x.DisplayOrder) + 1 : 1;
        foreach (var project in model.Projects.Where(x => x.Selected))
        {
            await adminContentService.SaveProjectAsync(new ProjectEditModel
            {
                TitleFr = project.TitleFr,
                TitleEn = project.TitleEn,
                SummaryFr = project.SummaryFr,
                SummaryEn = project.SummaryEn,
                DescriptionFr = project.DescriptionFr,
                DescriptionEn = project.DescriptionEn,
                TechStack = project.TechStack,
                DisplayOrder = nextProjectOrder++,
                IsFeatured = false,
                IsPublished = false
            }, cancellationToken);
            createdProjects++;
        }

        var existingExperience = await adminContentService.GetExperienceEntriesAsync(true, cancellationToken);
        var nextExperienceOrder = existingExperience.Any() ? existingExperience.Max(x => x.DisplayOrder) + 1 : 1;
        foreach (var experience in model.ExperienceEntries.Where(x => x.Selected))
        {
            await adminContentService.SaveExperienceEntryAsync(new ExperienceEditModel
            {
                RoleFr = experience.RoleFr,
                RoleEn = experience.RoleEn,
                Company = experience.Company,
                Location = string.IsNullOrWhiteSpace(experience.Location) ? "Remote" : experience.Location,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate,
                DescriptionFr = experience.DescriptionFr,
                DescriptionEn = experience.DescriptionEn,
                Technologies = experience.Technologies,
                DisplayOrder = nextExperienceOrder++,
                IsVisible = false
            }, cancellationToken);
            createdExperienceEntries++;
        }

        await cvImportService.MarkAppliedAsync(id, cancellationToken);
        if (model.DeleteFileAfterApply)
        {
            await cvImportService.DeleteImportAsync(id, cancellationToken);
        }

        TempData["AdminSuccess"] =
            $"CV import applied. Profile updated: {(model.Profile.Selected ? "yes" : "no")}. " +
            $"Draft services: {createdServices}. Draft projects: {createdProjects}. Hidden experience entries: {createdExperienceEntries}.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await cvImportService.DeleteImportAsync(id, cancellationToken);
        TempData["AdminSuccess"] = "CV import file deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static CvImportReviewViewModel MapDraft(CvImportDraft draft)
    {
        return new CvImportReviewViewModel
        {
            ImportId = draft.ImportId,
            OriginalFileName = draft.OriginalFileName,
            ExtractedTextPreview = draft.ExtractedTextPreview,
            Profile = new CvImportProfileSuggestionViewModel
            {
                Selected = true,
                SiteName = draft.Profile.SiteName,
                TaglineFr = draft.Profile.TaglineFr,
                TaglineEn = draft.Profile.TaglineEn,
                AboutSummaryFr = draft.Profile.AboutSummaryFr,
                AboutSummaryEn = draft.Profile.AboutSummaryEn,
                SkillsFr = draft.Profile.SkillsFr,
                SkillsEn = draft.Profile.SkillsEn,
                ContactEmail = draft.Profile.ContactEmail,
                Location = draft.Profile.Location,
                LinkedInUrl = draft.Profile.LinkedInUrl,
                GitHubUrl = draft.Profile.GitHubUrl
            },
            ExperienceEntries = draft.ExperienceEntries.Select(item => new CvImportExperienceSuggestionViewModel
            {
                Selected = true,
                RoleFr = item.RoleFr,
                RoleEn = item.RoleEn,
                Company = item.Company,
                Location = item.Location,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                DescriptionFr = item.DescriptionFr,
                DescriptionEn = item.DescriptionEn,
                Technologies = item.Technologies
            }).ToList(),
            Services = draft.Services.Select(item => new CvImportServiceSuggestionViewModel
            {
                Selected = true,
                TitleFr = item.TitleFr,
                TitleEn = item.TitleEn,
                SummaryFr = item.SummaryFr,
                SummaryEn = item.SummaryEn,
                DescriptionFr = item.DescriptionFr,
                DescriptionEn = item.DescriptionEn,
                IconClass = item.IconClass
            }).ToList(),
            Projects = draft.Projects.Select(item => new CvImportProjectSuggestionViewModel
            {
                Selected = false,
                TitleFr = item.TitleFr,
                TitleEn = item.TitleEn,
                SummaryFr = item.SummaryFr,
                SummaryEn = item.SummaryEn,
                DescriptionFr = item.DescriptionFr,
                DescriptionEn = item.DescriptionEn,
                TechStack = item.TechStack
            }).ToList()
        };
    }

    private void ValidateSelectedSections(CvImportReviewViewModel model)
    {
        if (model.Profile.Selected)
        {
            if (string.IsNullOrWhiteSpace(model.Profile.SiteName))
            {
                ModelState.AddModelError("Profile.SiteName", "Site name is required when applying profile suggestions.");
            }
        }

        for (var index = 0; index < model.Services.Count; index++)
        {
            var item = model.Services[index];
            if (!item.Selected)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.TitleEn) || string.IsNullOrWhiteSpace(item.SummaryEn) || string.IsNullOrWhiteSpace(item.DescriptionEn))
            {
                ModelState.AddModelError($"Services[{index}].TitleEn", "Selected services need a title, summary, and description.");
            }
        }

        for (var index = 0; index < model.ExperienceEntries.Count; index++)
        {
            var item = model.ExperienceEntries[index];
            if (!item.Selected)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.RoleEn) || string.IsNullOrWhiteSpace(item.Company) || string.IsNullOrWhiteSpace(item.DescriptionEn))
            {
                ModelState.AddModelError($"ExperienceEntries[{index}].RoleEn", "Selected experience entries need a role, company, and description.");
            }
        }

        for (var index = 0; index < model.Projects.Count; index++)
        {
            var item = model.Projects[index];
            if (!item.Selected)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(item.TitleEn) || string.IsNullOrWhiteSpace(item.SummaryEn) || string.IsNullOrWhiteSpace(item.DescriptionEn))
            {
                ModelState.AddModelError($"Projects[{index}].TitleEn", "Selected project suggestions need a title, summary, and description.");
            }
        }
    }

    private static string ChooseImported(string importedValue, string currentValue)
    {
        return string.IsNullOrWhiteSpace(importedValue) ? currentValue : importedValue.Trim();
    }
}
