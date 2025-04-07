using Bohio.Core.Entities;

namespace Bohio.Core.Interfaces;

public interface IEmailService
{
  Task SendEmailAsync(EmailOptions emailOptions);
  // string ReadEmailTemplate(TemplateEmailType templateName);
}
