using System.Text;
using System.Text.Json;
using IzinTakip.Models;

namespace IzinTakip.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _noRedirectClient;
    private readonly HttpClient _followRedirectClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string BaseUrl = "https://script.google.com/macros/s/AKfycby0JE-uoihEsZvm_01fCXWohuQEmn5fzmdAVZg93_bSbiS3-HMQ3CMNArF6ovJCrk1c/exec";

    public ApiService()
    {
        _noRedirectClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
        _followRedirectClient = new HttpClient();
    }

    private async Task<T> PostAsync<T>(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _noRedirectClient.PostAsync(BaseUrl, content);

        string responseBody;

        if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
        {
            var redirectUrl = response.Headers.Location?.ToString();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                if (!redirectUrl.StartsWith("http"))
                    redirectUrl = "https://script.google.com" + redirectUrl;

                var getResponse = await _followRedirectClient.GetAsync(redirectUrl);
                responseBody = await getResponse.Content.ReadAsStringAsync();
            }
            else
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }
        }
        else
        {
            responseBody = await response.Content.ReadAsStringAsync();
        }

        try
        {
            return JsonSerializer.Deserialize<T>(responseBody, JsonOptions)!;
        }
        catch (JsonException)
        {
            System.Diagnostics.Debug.WriteLine($"[ApiService] Beklenmeyen sunucu yanıtı: {responseBody[..Math.Min(500, responseBody.Length)]}");
            var errorResponse = new ApiResponse { Success = false, Message = "Sunucu yanıtı işlenemedi. Lütfen daha sonra tekrar deneyiniz veya destek ekibi ile iletişime geçiniz." };
            return (T)(object)errorResponse;
        }
    }

    public async Task<ApiResponse> VerifyEmailAsync(string email)
    {
        return await PostAsync<ApiResponse>(new { action = "verifyEmail", email });
    }

    public async Task<ApiResponse> ConfirmCodeAsync(string email, string code)
    {
        return await PostAsync<ApiResponse>(new { action = "confirmCode", email, code });
    }

    public async Task<ApiResponse> LoginOtpAsync(string email)
    {
        return await PostAsync<ApiResponse>(new { action = "loginOTP", email });
    }

    public async Task<ApiResponse<User>> ConfirmOtpAsync(string email, string otp)
    {
        return await PostAsync<ApiResponse<User>>(new { action = "confirmOTP", email, otp });
    }

    public async Task<ApiResponse> SubmitLeaveAsync(LeaveRequest request)
    {
        return await PostAsync<ApiResponse>(new
        {
            action = "submitLeave",
            email = request.Email,
            adSoyad = request.AdSoyad,
            birim = request.Birim,
            izinSuresi = request.IzinSuresi,
            baslangicTarihi = request.BaslangicTarihi,
            bitisTarihi = request.BitisTarihi,
            iseDonus = request.IseDonus,
            aciklama = request.Aciklama
        });
    }

    public async Task<ApiResponse<List<LeaveRequest>>> GetLeavesAsync(string email)
    {
        return await PostAsync<ApiResponse<List<LeaveRequest>>>(new { action = "getLeaves", email });
    }
}
