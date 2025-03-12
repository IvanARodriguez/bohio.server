
using FluentValidation;
using Bohio.Application.Mappings;
using Bohio.Application.Services;
using Bohio.Application.Validators;
using Bohio.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddAutoMapper(typeof(Profiles));


        return services;
    }
}
