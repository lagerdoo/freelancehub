using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/experience")]
public sealed class ExperienceController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort, string? dir, CancellationToken cancellationToken)
    {
        SetPageTitle("Experience");

        var items = await adminContentService.GetExperienceEntriesAsync(true, cancellationToken);
        var sortState = CreateSortState(sort, dir, "start");

        return View(new AdminListPageViewModel<ExperienceEntry>
        {
            Items = ApplySorting(items, sortState).ToList(),
            SortState = sortState
        });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        SetPageTitle("Create experience");
        return View("Edit", new ExperienceFormViewModel { IsVisible = true });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var entry = await adminContentService.GetExperienceEntryAsync(id, cancellationToken);
        if (entry is null)
        {
            return NotFound();
        }

        SetPageTitle("Edit experience");
        return View(new ExperienceFormViewModel
        {
            Id = entry.Id,
            RoleFr = entry.RoleFr,
            RoleEn = entry.RoleEn,
            Company = entry.Company,
            Location = entry.Location,
            StartDate = entry.StartDate,
            EndDate = entry.EndDate,
            DescriptionFr = entry.DescriptionFr,
            DescriptionEn = entry.DescriptionEn,
            Technologies = entry.Technologies,
            DisplayOrder = entry.DisplayOrder,
            IsVisible = entry.IsVisible
        });
    }

    [HttpPost("edit/{id?}")]
    public async Task<IActionResult> Edit(ExperienceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            SetPageTitle(model.Id.HasValue ? "Edit experience" : "Create experience");
            return View(model);
        }

        await adminContentService.SaveExperienceEntryAsync(new ExperienceEditModel
        {
            Id = model.Id,
            RoleFr = model.RoleFr,
            RoleEn = model.RoleEn,
            Company = model.Company,
            Location = model.Location,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            DescriptionFr = model.DescriptionFr,
            DescriptionEn = model.DescriptionEn,
            Technologies = model.Technologies,
            DisplayOrder = model.DisplayOrder,
            IsVisible = model.IsVisible
        }, cancellationToken);

        TempData["AdminSuccess"] = "Experience entry saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await adminContentService.DeleteExperienceEntryAsync(id, cancellationToken);
        TempData["AdminSuccess"] = "Experience entry deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static AdminSortState CreateSortState(string? sort, string? direction, string defaultSort)
    {
        var normalizedDirection = string.Equals(direction, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        var normalizedSort = string.IsNullOrWhiteSpace(sort) ? defaultSort : sort.Trim();

        return new AdminSortState
        {
            Sort = normalizedSort,
            Direction = normalizedDirection
        };
    }

    private static IOrderedEnumerable<ExperienceEntry> ApplySorting(IEnumerable<ExperienceEntry> items, AdminSortState sortState)
    {
        return (sortState.Sort.ToLowerInvariant(), sortState.Direction) switch
        {
            ("role", "asc") => items.OrderBy(x => x.RoleEn).ThenBy(x => x.StartDate),
            ("role", _) => items.OrderByDescending(x => x.RoleEn).ThenByDescending(x => x.StartDate),
            ("company", "asc") => items.OrderBy(x => x.Company).ThenByDescending(x => x.StartDate),
            ("company", _) => items.OrderByDescending(x => x.Company).ThenByDescending(x => x.StartDate),
            ("period", "asc") => items.OrderBy(x => x.EndDate ?? DateOnly.MaxValue).ThenBy(x => x.StartDate),
            ("period", _) => items.OrderByDescending(x => x.EndDate ?? DateOnly.MaxValue).ThenByDescending(x => x.StartDate),
            ("visible", "asc") => items.OrderBy(x => x.IsVisible).ThenBy(x => x.RoleEn),
            ("visible", _) => items.OrderByDescending(x => x.IsVisible).ThenBy(x => x.RoleEn),
            ("start", "asc") => items.OrderBy(x => x.StartDate).ThenBy(x => x.DisplayOrder),
            _ => items.OrderByDescending(x => x.StartDate).ThenBy(x => x.DisplayOrder)
        };
    }
}
