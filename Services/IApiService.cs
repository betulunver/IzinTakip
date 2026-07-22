using IzinTakip.Models;

namespace IzinTakip.Services;

public interface IApiService
{
    Task<ApiResponse> VerifyEmailAsync(string email);
    Task<ApiResponse> ConfirmCodeAsync(string email, string code);
    Task<ApiResponse> LoginOtpAsync(string email);
    Task<ApiResponse<User>> ConfirmOtpAsync(string email, string otp);
    Task<ApiResponse> SubmitLeaveAsync(LeaveRequest request);
    Task<ApiResponse<List<LeaveRequest>>> GetLeavesAsync(string email);
}
