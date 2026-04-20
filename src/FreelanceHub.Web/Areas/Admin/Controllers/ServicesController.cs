using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/services")]
public sealed class ServicesController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort, string? dir, CancellationToken cancellationToken)
    {
        SetPageTitle("Services");

        var items = await adminContentService.GetServicesAsync(true, cancellationToken);
        var sortState = CreateSortState(sort, dir, "order");

        return View(new AdminListPageViewModel<Service>
        {
            Items = ApplySorting(items, sortState).ToList(),
            SortState = sortState
        });
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        SetPageTitle("Create service");
        return View("Edit", new ServiceFormViewModel { IsPublished = true });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var service = await adminContentService.GetServiceAsync(id, cancellationToken);
        if (service is null)
        {
            return NotFound();
        }

        SetPageTitle("Edit service");
        return View(new ServiceFormViewModel
        {
            Id = service.Id,
            TitleFr = service.TitleFr,
            TitleEn = service.TitleEn,
            SummaryFr = service.SummaryFr,
            SummaryEn = service.SummaryEn,
            DescriptionFr = service.DescriptionFr,
            DescriptionEn = service.DescriptionEn,
            IconClass = service.IconClass,
            ImagePath = service.ImagePath,
            DisplayOrder = service.DisplayOrder,
            IsPublished = service.IsPublished,
            Slug = service.Slug
        });
    }

    [HttpPost("edit/{id?}")]
    public async Task<IActionResult> Edit(ServiceFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            SetPageTitle(model.Id.HasValue ? "Edit service" : "Create service");
            return View(model);
        }

        await adminContentService.SaveServiceAsync(new ServiceEditModel
        {
            Id = model.Id,
            TitleFr = model.TitleFr,
            TitleEn = model.TitleEn,
            SummaryFr = model.SummaryFr,
            SummaryEn = model.SummaryEn,
            DescriptionFr = model.DescriptionFr,
            DescriptionEn = model.DescriptionEn,
            IconClass = model.IconClass,
            ImagePath = model.ImagePath,
            DisplayOrder = model.DisplayOrder,
            IsPublished = model.IsPublished,
            Slug = model.Slug
        }, cancellationToken);

        TempData["AdminSuccess"] = "Service saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await adminContentService.DeleteServiceAsync(id, cancellationToken);
        TempData["AdminSuccess"] = "Service deleted.";
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

    private static IOrderedEnumerable<Service> ApplySorting(IEnumerable<Service> items, AdminSortState sortState)
    {
        return (sortState.Sort.ToLowerInvariant(), sortState.Direction) switch
        {
            ("titlefr", "desc") => items.OrderByDescending(x => x.TitleFr).ThenBy(x => x.TitleEn),
            ("titlefr", _) => items.OrderBy(x => x.TitleFr).ThenBy(x => x.TitleEn),
            ("titleen", "desc") => items.OrderByDescending(x => x.TitleEn).ThenBy(x => x.TitleFr),
            ("titleen", _) => items.OrderBy(x => x.TitleEn).ThenBy(x => x.TitleFr),
            ("icon", "desc") => items.OrderByDescending(x => x.IconClass).ThenBy(x => x.TitleEn),
            ("icon", _) => items.OrderBy(x => x.IconClass).ThenBy(x => x.TitleEn),
            ("status", "desc") => items.OrderByDescending(x => x.IsPublished).ThenBy(x => x.TitleEn),
            ("status", _) => items.OrderBy(x => x.IsPublished).ThenBy(x => x.TitleEn),
            ("order", "desc") => items.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.TitleEn),
            _ => items.OrderBy(x => x.DisplayOrder).ThenBy(x => x.TitleEn)
        };
    }
}
