using Homespirations.Core.Entities;

namespace Homespirations.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailOptions emailOptions);
}
