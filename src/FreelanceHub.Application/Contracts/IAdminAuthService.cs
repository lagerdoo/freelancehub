using FreelanceHub.Application.Models;

namespace FreelanceHub.Application.Contracts;

public interface IAdminAuthService
{
    Task<AdminIdentity?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
