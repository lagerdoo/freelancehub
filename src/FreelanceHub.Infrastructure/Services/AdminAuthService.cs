using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Services;

public sealed class AdminAuthService(FreelanceHubDbContext dbContext, IPasswordHasher passwordHasher) : IAdminAuthService
{
    public async Task<AdminIdentity?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = request.UsernameOrEmail.Trim().ToLowerInvariant();

        var user = await dbContext.AdminUsers.FirstOrDefaultAsync(
            x => x.IsActive && (x.Username.ToLower() == normalized || x.Email.ToLower() == normalized),
            cancellationToken);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AdminIdentity
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName
        };
    }
}
