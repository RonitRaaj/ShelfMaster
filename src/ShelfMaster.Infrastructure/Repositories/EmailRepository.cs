using ShelfMaster.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace ShelfMaster.Infrastructure.Repositories;

public class EmailRepository : IEmailRepository
{

    private readonly IConfiguration _configuration;
    public EmailRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var fromEmail = _configuration["EmailSettings:SmtpUser"];
        var password = _configuration["EmailSettings:SmtpPassword"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = false,
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail!),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}