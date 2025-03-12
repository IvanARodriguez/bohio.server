using Homespirations.Core.Entities;
using Homespirations.Core.Types;

namespace Homespirations.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailOptions emailOptions);
    string ReadEmailTemplate(TemplateEmailType templateName);
}
