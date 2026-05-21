namespace ShelfMaster.Application.Interfaces;

public interface IEmailRepository
{
    Task SendEmailAsync(string to, string subject, string body);
}