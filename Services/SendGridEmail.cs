using System.ComponentModel.DataAnnotations;
using IdentityApp.Helpers;
using IdentityApp.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityApp.Services;

public class SendGridEmail : ISendGridEmail
{
    private readonly ILogger<SendGridEmail> _logger;
    public AuthMessageSenderOptions Options { get; set; }
    public SendGridEmail(IOptions<AuthMessageSenderOptions> options, ILogger<SendGridEmail> logger)
    {
        _logger = logger;
        Options = options.Value;
    }

    public async Task Execute(string apiKey, string subject, string messsage, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress("seminenkovs@gmail.com"),
            Subject = subject,
            PlainTextContent = messsage,
            HtmlContent = messsage
        };
        msg.AddTo(new EmailAddress(toEmail));
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        var dummy = response.StatusCode;
        var dummy2 = response.Headers;
        _logger.LogInformation(response.IsSuccessStatusCode
                ? $"Email to {toEmail} queued successfully": $"Failure email to {toEmail}");

    }
}