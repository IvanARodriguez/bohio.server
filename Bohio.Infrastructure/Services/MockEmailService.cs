using Bohio.Core.Entities;
using Bohio.Core.Interfaces;

namespace Bohio.Infrastructure.Services;

public class MockEmailService : IEmailService
{
  public static List<EmailOptions> SentEmails { get; } = [];
  public Task SendEmailAsync(EmailOptions emailOptions)
  {
    SentEmails.Add(emailOptions);
    Console.WriteLine($"[MockEmail] To: {emailOptions.To}, Subject: {emailOptions.Subject}");
    return Task.CompletedTask;
  }
}
