using SubspaceStats.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace SubspaceStats.Services;

public class EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
    ILogger<EmailSender> logger) : IEmailSender<SubspaceStatsUser>
{
    private readonly ILogger _logger = logger;
    
    public AuthMessageSenderOptions Options { get; } = optionsAccessor.Value;   

    public Task SendConfirmationLinkAsync(SubspaceStatsUser user, string email,
       string confirmationLink) => SendEmailAsync(email, "Confirm your email",
       "<html lang=\"en\"><head></head><body>Please confirm your account by " +
       $"<a href='{confirmationLink}'>clicking here</a>.</body></html>");

    public Task SendPasswordResetLinkAsync(SubspaceStatsUser user, string email,
        string resetLink) => SendEmailAsync(email, "Reset your password",
        "<html lang=\"en\"><head></head><body>Please reset your password by " +
        $"<a href='{resetLink}'>clicking here</a>.</body></html>");

    public Task SendPasswordResetCodeAsync(SubspaceStatsUser user, string email,
        string resetCode) => SendEmailAsync(email, "Reset your password",
        "<html lang=\"en\"><head></head><body>Please reset your password " +
        $"using the following code:<br>{resetCode}</body></html>");


    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (Options.SendGridOverSmtp)
        {
            if (string.IsNullOrEmpty(Options.SendGridKey))
            {
                throw new Exception("Null SendGridKey");
            }
            await Execute(Options.SendGridKey, subject, message, toEmail);
        } 
        else
        {
            if (string.IsNullOrEmpty(Options.SmtpHost) || string.IsNullOrEmpty(Options.SmtpUsername) || string.IsNullOrEmpty(Options.SmtpPassword))
            {
                throw new Exception("Missing SMTP Config");
            }
            await ExecuteSmtp(toEmail, subject, message);
        }
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(Options.SmtpFromEmail!, Options.SmtpFromName!),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode
                               ? $"Email to {toEmail} queued successfully!"
                               : $"Failure Email to {toEmail}");
    }

    public async Task ExecuteSmtp(string email, string subject, string htmlMessage)
    {
        SmtpClient smtpClient = new();
        smtpClient.UseDefaultCredentials = Options.SmtpUseDefaultCredentials;
        smtpClient.EnableSsl = Options.SmtpSSL;
        smtpClient.Host = Options.SmtpHost!;
        smtpClient.Port = Options.SmtpPort;
        smtpClient.Credentials = new System.Net.NetworkCredential(Options.SmtpUsername, Options.SmtpPassword);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(Options.SmtpFromEmail!, Options.SmtpFromName!),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine($"Successful Email to {email}");
            _logger.LogInformation($"Successful Email to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failure Email to {email}", ex.Message.ToString());
            _logger.LogInformation($"Failure Email to {email}", ex.Message.ToString());
        }
    }
}