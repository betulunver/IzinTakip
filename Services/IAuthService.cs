using IzinTakip.Models;

namespace IzinTakip.Services;

public interface IAuthService
{
    Task<bool> IsLoggedInAsync();
    Task<User?> GetCurrentUserAsync();
    Task SaveSessionAsync(User user);
    Task ClearSessionAsync();
}
