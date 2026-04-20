using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/messages")]
public sealed class ContactMessagesController(IAdminContentService adminContentService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? sort, string? dir, CancellationToken cancellationToken)
    {
        SetPageTitle("Contact messages");

        var items = await adminContentService.GetContactMessagesAsync(cancellationToken);
        var sortState = CreateSortState(sort, dir, "date");

        return View(new AdminListPageViewModel<ContactMessage>
        {
            Items = ApplySorting(items, sortState).ToList(),
            SortState = sortState
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var message = await adminContentService.GetContactMessageAsync(id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        if (!message.IsRead)
        {
            await adminContentService.UpdateContactMessageAsync(new ContactMessageStatusUpdateModel
            {
                Id = message.Id,
                IsRead = true,
                IsArchived = message.IsArchived
            }, cancellationToken);

            message.IsRead = true;
        }

        SetPageTitle("Message details");
        return View(BuildDetailsViewModel(message));
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, bool isRead, bool isArchived, CancellationToken cancellationToken)
    {
        await adminContentService.UpdateContactMessageAsync(new ContactMessageStatusUpdateModel
        {
            Id = id,
            IsRead = isRead,
            IsArchived = isArchived
        }, cancellationToken);

        TempData["AdminSuccess"] = "Message updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("{id:guid}/reply")]
    public async Task<IActionResult> Reply(Guid id, [Bind(Prefix = "ReplyForm")] ContactMessageReplyViewModel model, CancellationToken cancellationToken)
    {
        var message = await adminContentService.GetContactMessageAsync(id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        if (id != model.MessageId)
        {
            ModelState.AddModelError(string.Empty, "The reply draft no longer matches the selected message.");
        }

        if (!ModelState.IsValid)
        {
            SetPageTitle("Message details");
            return View("Details", BuildDetailsViewModel(message, model));
        }

        try
        {
            await adminContentService.SendContactReplyAsync(new ContactMessageReplyModel
            {
                Id = id,
                ToEmail = model.ToEmail,
                Subject = model.Subject,
                Body = model.Body
            }, cancellationToken);

            TempData["AdminSuccess"] = "Reply sent successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidOperationException exception)
        {
            TempData["AdminError"] = exception.Message;
        }
        catch
        {
            TempData["AdminError"] = "The reply could not be sent. Review your SMTP configuration and try again.";
        }

        var refreshedMessage = await adminContentService.GetContactMessageAsync(id, cancellationToken) ?? message;
        SetPageTitle("Message details");
        return View("Details", BuildDetailsViewModel(refreshedMessage, model));
    }

    [HttpPost("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await adminContentService.DeleteContactMessageAsync(id, cancellationToken);
        TempData["AdminSuccess"] = "Message deleted permanently.";
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

    private static IOrderedEnumerable<ContactMessage> ApplySorting(IEnumerable<ContactMessage> items, AdminSortState sortState)
    {
        return (sortState.Sort.ToLowerInvariant(), sortState.Direction) switch
        {
            ("name", "asc") => items.OrderBy(x => x.Name).ThenByDescending(x => x.CreatedAtUtc),
            ("name", _) => items.OrderByDescending(x => x.Name).ThenByDescending(x => x.CreatedAtUtc),
            ("email", "asc") => items.OrderBy(x => x.Email).ThenByDescending(x => x.CreatedAtUtc),
            ("email", _) => items.OrderByDescending(x => x.Email).ThenByDescending(x => x.CreatedAtUtc),
            ("subject", "asc") => items.OrderBy(x => x.Subject).ThenByDescending(x => x.CreatedAtUtc),
            ("subject", _) => items.OrderByDescending(x => x.Subject).ThenByDescending(x => x.CreatedAtUtc),
            ("status", "asc") => items.OrderBy(x => x.IsArchived).ThenBy(x => x.IsRead).ThenByDescending(x => x.CreatedAtUtc),
            ("status", _) => items.OrderByDescending(x => x.IsArchived).ThenByDescending(x => x.IsRead).ThenByDescending(x => x.CreatedAtUtc),
            ("followup", "asc") => items.OrderBy(x => x.FollowUpStatus).ThenByDescending(x => x.CreatedAtUtc),
            ("followup", _) => items.OrderByDescending(x => x.FollowUpStatus).ThenByDescending(x => x.CreatedAtUtc),
            ("date", "asc") => items.OrderBy(x => x.CreatedAtUtc).ThenBy(x => x.Name),
            _ => items.OrderByDescending(x => x.CreatedAtUtc).ThenBy(x => x.Name)
        };
    }

    private static ContactMessageDetailsViewModel BuildDetailsViewModel(ContactMessage message, ContactMessageReplyViewModel? replyForm = null)
    {
        return new ContactMessageDetailsViewModel
        {
            Message = message,
            ReplyForm = replyForm ?? new ContactMessageReplyViewModel
            {
                MessageId = message.Id,
                ToEmail = message.Email,
                Subject = GetReplySubject(message.Subject)
            }
        };
    }

    private static string GetReplySubject(string subject)
    {
        return subject.StartsWith("Re:", StringComparison.OrdinalIgnoreCase)
            ? subject
            : $"Re: {subject}";
    }
}
