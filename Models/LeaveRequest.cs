namespace IzinTakip.Models;

public class LeaveRequest
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AdSoyad { get; set; } = string.Empty;
    public string Birim { get; set; } = string.Empty;
    public string IzinSuresi { get; set; } = string.Empty;
    public string BaslangicTarihi { get; set; } = string.Empty;
    public string BitisTarihi { get; set; } = string.Empty;
    public string IseDonus { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string OlusturmaTarihi { get; set; } = string.Empty;
    public string Durum { get; set; } = "Beklemede";
}
