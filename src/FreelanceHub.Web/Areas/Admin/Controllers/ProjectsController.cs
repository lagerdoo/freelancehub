using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/projects")]
public sealed class ProjectsController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort, string? dir, CancellationToken cancellationToken)
    {
        SetPageTitle("Projects");

        var items = await adminContentService.GetProjectsAsync(true, cancellationToken);
        var sortState = CreateSortState(sort, dir, "order");

        return View(new AdminListPageViewModel<Project>
        {
            Items = ApplySorting(items, sortState).ToList(),
            SortState = sortState
        });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        SetPageTitle("Create project");
        return View("Edit", new ProjectFormViewModel { IsPublished = true, IsFeatured = true });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var project = await adminContentService.GetProjectAsync(id, cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        SetPageTitle("Edit project");
        return View(new ProjectFormViewModel
        {
            Id = project.Id,
            TitleFr = project.TitleFr,
            TitleEn = project.TitleEn,
            SummaryFr = project.SummaryFr,
            SummaryEn = project.SummaryEn,
            DescriptionFr = project.DescriptionFr,
            DescriptionEn = project.DescriptionEn,
            TechStack = project.TechStack,
            MainImagePath = project.MainImagePath,
            ExternalLink = project.ExternalLink,
            IsFeatured = project.IsFeatured,
            DisplayOrder = project.DisplayOrder,
            IsPublished = project.IsPublished,
            Slug = project.Slug
        });
    }

    [HttpPost("edit/{id?}")]
    public async Task<IActionResult> Edit(ProjectFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            SetPageTitle(model.Id.HasValue ? "Edit project" : "Create project");
            return View(model);
        }

        await adminContentService.SaveProjectAsync(new ProjectEditModel
        {
            Id = model.Id,
            TitleFr = model.TitleFr,
            TitleEn = model.TitleEn,
            SummaryFr = model.SummaryFr,
            SummaryEn = model.SummaryEn,
            DescriptionFr = model.DescriptionFr,
            DescriptionEn = model.DescriptionEn,
            TechStack = model.TechStack,
            MainImagePath = model.MainImagePath,
            ExternalLink = model.ExternalLink,
            IsFeatured = model.IsFeatured,
            DisplayOrder = model.DisplayOrder,
            IsPublished = model.IsPublished,
            Slug = model.Slug
        }, cancellationToken);

        TempData["AdminSuccess"] = "Project saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await adminContentService.DeleteProjectAsync(id, cancellationToken);
        TempData["AdminSuccess"] = "Project deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static AdminSortState CreateSortState(string? sort, string? direction, string defaultSort)
    {
        var normalizedDirection = string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        var normalizedSort = string.IsNullOrWhiteSpace(sort) ? defaultSort : sort.Trim();

        return new AdminSortState
        {
            Sort = normalizedSort,
            Direction = normalizedDirection
        };
    }

    private static IOrderedEnumerable<Project> ApplySorting(IEnumerable<Project> items, AdminSortState sortState)
    {
        return (sortState.Sort.ToLowerInvariant(), sortState.Direction) switch
        {
            ("titleen", "desc") => items.OrderByDescending(x => x.TitleEn).ThenBy(x => x.DisplayOrder),
            ("titleen", _) => items.OrderBy(x => x.TitleEn).ThenBy(x => x.DisplayOrder),
            ("featured", "desc") => items.OrderByDescending(x => x.IsFeatured).ThenBy(x => x.TitleEn),
            ("featured", _) => items.OrderBy(x => x.IsFeatured).ThenBy(x => x.TitleEn),
            ("status", "desc") => items.OrderByDescending(x => x.IsPublished).ThenBy(x => x.TitleEn),
            ("status", _) => items.OrderBy(x => x.IsPublished).ThenBy(x => x.TitleEn),
            ("order", "desc") => items.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.TitleEn),
            _ => items.OrderBy(x => x.DisplayOrder).ThenBy(x => x.TitleEn)
        };
    }
}
