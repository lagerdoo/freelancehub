using FreelanceHub.Application.Contracts;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin")]
public sealed class DashboardController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort, string? dir, CancellationToken cancellationToken)
    {
        SetPageTitle("Dashboard");
        var summary = await adminContentService.GetDashboardAsync(cancellationToken);
        var sortState = CreateSortState(sort, dir, "date");

        return View(new DashboardViewModel
        {
            Summary = summary,
            RecentMessagesTable = new AdminListPageViewModel<ContactMessage>
            {
                Items = ApplySorting(summary.RecentMessages, sortState).ToList(),
                SortState = sortState
            }
        });
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

    private static IOrderedEnumerable<ContactMessage> ApplySorting(IEnumerable<ContactMessage> items, AdminSortState sortState)
    {
        return (sortState.Sort.ToLowerInvariant(), sortState.Direction) switch
        {
            ("name", "asc") => items.OrderBy(x => x.Name).ThenByDescending(x => x.CreatedAtUtc),
            ("name", _) => items.OrderByDescending(x => x.Name).ThenByDescending(x => x.CreatedAtUtc),
            ("subject", "asc") => items.OrderBy(x => x.Subject).ThenByDescending(x => x.CreatedAtUtc),
            ("subject", _) => items.OrderByDescending(x => x.Subject).ThenByDescending(x => x.CreatedAtUtc),
            ("date", "asc") => items.OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Name),
            _ => items.OrderByDescending(x => x.CreatedAtUtc).ThenBy(x => x.Name)
        };
    }
}
