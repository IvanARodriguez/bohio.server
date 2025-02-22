using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using DotNetEnv;
using Homespirations.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Homespirations.Infrastructure.Services;

public class SesEmailService : IEmailService
{
    private readonly ILogger<SesEmailService> _logger;
    private readonly IAmazonSimpleEmailServiceV2 _sesClient;
    private readonly string _senderEmail;

    public SesEmailService(ILogger<SesEmailService> logger)
    {
        _logger = logger;

        Env.Load();

        // Load AWS credentials from environment variables
        var awsAccessKey = Environment.GetEnvironmentVariable("AWS_SES_USER");
        var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SES_PASS");
        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-2";

        if (string.IsNullOrEmpty(awsAccessKey) || string.IsNullOrEmpty(awsSecretKey))
        {
            throw new InvalidOperationException("AWS SES credentials are missing. Set AWS_SES_USER and AWS_SES_PASS.");
        }

        // Initialize SES client with credentials
        var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        var config = new AmazonSimpleEmailServiceV2Config { RegionEndpoint = RegionEndpoint.GetBySystemName(awsRegion) };

        _sesClient = new AmazonSimpleEmailServiceV2Client(credentials, config);
        _senderEmail = Environment.GetEnvironmentVariable("AWS_SES_SENDER") ?? "no-reply@homespirations.com";
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var request = new SendEmailRequest
        {
            FromEmailAddress = _senderEmail,
            Destination = new Destination { ToAddresses = new List<string> { to } },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content { Data = subject },
                    Body = new Body { Html = new Content { Data = body } }
                }
            }
        };

        try
        {
            await _sesClient.SendEmailAsync(request);
            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
        }
    }
}
