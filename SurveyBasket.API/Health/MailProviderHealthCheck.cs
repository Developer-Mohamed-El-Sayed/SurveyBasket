namespace SurveyBasket.API.Health;

public class MailProviderHealthCheck(IOptions<MailSettings> options) : IHealthCheck
{
    private readonly MailSettings _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtp = new SmtpClient();
            smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
            smtp.Authenticate(_options.Mail, _options.Password, cancellationToken);
            return await Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception exception)
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy(exception: exception));
        }
    }
}
