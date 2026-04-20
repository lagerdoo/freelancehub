using System.Net;
using System.Net.Mail;
using System.Text;
using FreelanceHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreelanceHub.Infrastructure.Services;

public sealed class EmailDeliveryService(IOptions<SmtpOptions> smtpOptions, ILogger<EmailDeliveryService> logger)
{
    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        var options = smtpOptions.Value;
        if (!options.Enabled)
        {
            throw new InvalidOperationException("Outbound email is not configured. Enable the Smtp section before sending replies.");
        }

        if (string.IsNullOrWhiteSpace(options.Host) || string.IsNullOrWhiteSpace(options.FromEmail))
        {
            throw new InvalidOperationException("SMTP host and from email must be configured before sending replies.");
        }

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.UseSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            client.Credentials = new NetworkCredential(options.Username, options.Password);
        }

        using var message = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName),
            Subject = subject,
            SubjectEncoding = Encoding.UTF8,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = false
        };

        message.To.Add(new MailAddress(toEmail));

        using var registration = cancellationToken.Register(client.SendAsyncCancel);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to send contact reply email to {Email}.", toEmail);
            throw;
        }
    }
}
