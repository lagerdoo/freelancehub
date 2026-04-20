using System.ComponentModel.DataAnnotations;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Web.ViewModels.Admin;

public sealed class AdminSortState
{
    public string Sort { get; init; } = string.Empty;
    public string Direction { get; init; } = "asc";

    public bool IsActive(string sortKey) =>
        string.Equals(Sort, sortKey, StringComparison.OrdinalIgnoreCase);

    public bool IsDescending(string sortKey) =>
        IsActive(sortKey) && string.Equals(Direction, "desc", StringComparison.OrdinalIgnoreCase);

    public string GetNextDirection(string sortKey) =>
        IsDescending(sortKey) ? "asc" : "desc";
}

public sealed class AdminSortHeaderViewModel
{
    public string Label { get; init; } = string.Empty;
    public string SortKey { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public AdminSortState SortState { get; init; } = new();

    public bool IsActive => SortState.IsActive(SortKey);
    public bool IsDescending => SortState.IsDescending(SortKey);
    public string IconClass => !IsActive
        ? "fa-solid fa-sort text-secondary"
        : IsDescending
            ? "fa-solid fa-arrow-down-wide-short"
            : "fa-solid fa-arrow-up-wide-short";
}

public sealed class AdminListPageViewModel<TItem>
{
    public IReadOnlyList<TItem> Items { get; init; } = Array.Empty<TItem>();
    public AdminSortState SortState { get; init; } = new();
}

public sealed class ContactMessageReplyViewModel
{
    public Guid MessageId { get; set; }

    [Required, EmailAddress, StringLength(256)]
    public string ToEmail { get; set; } = string.Empty;

    [Required, StringLength(180)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 10)]
    public string Body { get; set; } = string.Empty;
}

public sealed class ContactMessageDetailsViewModel
{
    public ContactMessage Message { get; init; } = new();
    public ContactMessageReplyViewModel ReplyForm { get; init; } = new();
}
