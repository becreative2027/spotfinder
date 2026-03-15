using Microsoft.Extensions.Logging;

namespace SpotFinder.AuthService.Business.Services;

/// <summary>
/// Geliştirme ortamı için OTP kodunu SMS göndermek yerine loglayan mock servis.
/// Production'da TwilioSmsService kullanılır.
/// </summary>
public class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;

    public MockSmsService(ILogger<MockSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendOtpAsync(string phoneNumber, string code, CancellationToken ct = default)
    {
        _logger.LogWarning("[MOCK SMS] {PhoneNumber} numarasına OTP gönderildi: {Code}", phoneNumber, code);
        return Task.CompletedTask;
    }
}
