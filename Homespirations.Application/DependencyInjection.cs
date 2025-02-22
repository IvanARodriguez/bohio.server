
using FluentValidation;
using Homespirations.Application.Services;
using Homespirations.Application.Validators;
using Homespirations.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Homespirations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<HomeSpaceService>();
        services.AddScoped<MediaServices>();
        services.AddSingleton<IValidator<HomeSpace>, HomeSpaceValidator>();
        services.AddSingleton<IValidator<Media>, MediaValidator>();
        services.AddSingleton<IValidator<FormFile>, FormFileValidator>();

        return services;
    }
}
