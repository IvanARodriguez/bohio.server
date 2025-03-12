using Bohio.Core.Entities;
using Bohio.Core.Types;

namespace Bohio.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailOptions emailOptions);
    string ReadEmailTemplate(TemplateEmailType templateName);
}
