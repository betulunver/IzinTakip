namespace IzinTakip.Models;

public class User
{
    public string Email { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string Rol { get; set; } = "staff";
    public bool Dogrulandi { get; set; }
}
