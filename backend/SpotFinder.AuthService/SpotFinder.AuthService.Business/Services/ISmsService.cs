namespace SpotFinder.AuthService.Business.Services;

public interface ISmsService
{
    Task SendOtpAsync(string phoneNumber, string code, CancellationToken ct = default);
}
