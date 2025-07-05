using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Shared.DTO.User;

namespace Incidenten.API.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByEmail(string? email);
    Task<User?> GetAnonymUser();
    bool IsUserEmailUnique(string? email);
    bool IsPasswordValid(User? user, string password);
    bool IsPrivelegedRole(User user);
    Task<bool> IsPrivelegedRoleAsync(string email);
    UserRole GetUserRole(string email);
    string GenerateToken(User user);
    Task<User> CreateUser(SignUpRequest request);
    Task ToggleUserNotifications(User user);
}