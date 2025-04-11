using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Bohio.Core.Entities;
using Bohio.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Bohio.Infrastructure.Services;

public class SesEmailService(ILogger<SesEmailService> logger, IAmazonSimpleEmailService emailService) : IEmailService
{
  private readonly ILogger<SesEmailService> _logger = logger;
  private readonly IAmazonSimpleEmailService _emailService = emailService;

  public async Task SendEmailAsync(EmailOptions emailOption)
  {
    var mailBody = new Body
    {
      Html = new Content { Charset = "UTF-8", Data = emailOption.HtmlBody },
      Text = new Content { Charset = "UTF-8", Data = emailOption.TextBody }
    };

    var message = new Message(new Content(emailOption.Subject), mailBody);
    var request = new SendEmailRequest(emailOption.From, new Destination([emailOption.To]), message);

    try
    {
      await _emailService.SendEmailAsync(request);
      _logger.LogInformation("Email sent successfully to {To}", emailOption.To);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to send email to {To}", emailOption.To);
      throw;
    }
  }
}
