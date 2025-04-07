
using FluentValidation;
using Bohio.Application.Mappings;
using Bohio.Application.Services;
using Bohio.Application.Validators;
using Bohio.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Bohio.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace Bohio.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddScoped<HomeSpaceService>();
    services.AddScoped<MediaServices>();
    services.AddScoped<AuthService>();
    services.AddSingleton<IValidator<HomeSpace>, HomeSpaceValidator>();
    services.AddSingleton<IValidator<Media>, MediaValidator>();
    services.AddSingleton<IValidator<FormFile>, FormFileValidator>();
    services.AddSingleton<IValidator<LoginRequest>, LoginValidator>();
    services.AddSingleton<IValidator<RegisterRequest>, RegisterValidator>();
    services.AddSingleton<IValidator<ConfirmEmailRequest>, ConfirmEmailValidator>();
    services.AddAutoMapper(typeof(Profiles));

    services.AddLogging();

    services.AddSingleton(provider =>
    {
      var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
      return loggerFactory.CreateLogger("Application");
    });

    return services;
  }
}
