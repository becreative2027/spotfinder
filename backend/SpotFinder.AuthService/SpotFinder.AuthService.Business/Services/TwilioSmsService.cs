using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SpotFinder.AuthService.Business.Services;

public class TwilioSmsService : ISmsService
{
    private readonly string _fromNumber;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger)
    {
        _logger = logger;

        var accountSid = configuration["Twilio:AccountSid"]
            ?? throw new InvalidOperationException("Twilio:AccountSid yapılandırması eksik.");
        var authToken = configuration["Twilio:AuthToken"]
            ?? throw new InvalidOperationException("Twilio:AuthToken yapılandırması eksik.");
        _fromNumber = configuration["Twilio:FromNumber"]
            ?? throw new InvalidOperationException("Twilio:FromNumber yapılandırması eksik.");

        TwilioClient.Init(accountSid, authToken);
    }

    public async Task SendOtpAsync(string phoneNumber, string code, CancellationToken ct = default)
    {
        var message = await MessageResource.CreateAsync(
            to: new Twilio.Types.PhoneNumber(phoneNumber),
            from: new Twilio.Types.PhoneNumber(_fromNumber),
            body: $"SpotFinder doğrulama kodunuz: {code}. Bu kod 5 dakika geçerlidir."
        );

        _logger.LogInformation("OTP SMS gönderildi. SID: {MessageSid}, Numara: {PhoneNumber}",
            message.Sid, phoneNumber);
    }
}
