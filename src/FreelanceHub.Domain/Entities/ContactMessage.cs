using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class ContactMessage : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
    public ContactFollowUpStatus FollowUpStatus { get; set; } = ContactFollowUpStatus.AwaitingReply;
    public DateTime? LastReplyAtUtc { get; set; }
    public string? LastReplySubject { get; set; }
    public string? LastReplyPreview { get; set; }
    public string? LastReplyError { get; set; }
}
