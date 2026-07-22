using IzinTakip.Models;

namespace IzinTakip.Services;

public class AuthService : IAuthService
{
    private const string EmailKey = "user_email";
    private const string NameKey = "user_name";
    private const string RoleKey = "user_role";
    private const string LoggedInKey = "is_logged_in";

    public async Task<bool> IsLoggedInAsync()
    {
        var val = await SecureStorage.GetAsync(LoggedInKey);
        return val == "true";
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var loggedIn = await IsLoggedInAsync();
        if (!loggedIn) return null;

        return new User
        {
            Email = await SecureStorage.GetAsync(EmailKey) ?? string.Empty,
            AdSoyad = await SecureStorage.GetAsync(NameKey) ?? string.Empty,
            Rol = await SecureStorage.GetAsync(RoleKey) ?? "staff"
        };
    }

    public async Task SaveSessionAsync(User user)
    {
        await SecureStorage.SetAsync(EmailKey, user.Email);
        await SecureStorage.SetAsync(NameKey, user.AdSoyad);
        await SecureStorage.SetAsync(RoleKey, user.Rol);
        await SecureStorage.SetAsync(LoggedInKey, "true");
    }

    public async Task ClearSessionAsync()
    {
        SecureStorage.RemoveAll();
        await Task.CompletedTask;
    }
}
