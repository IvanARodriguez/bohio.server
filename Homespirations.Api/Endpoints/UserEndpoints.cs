using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Homespirations.Application;
using Homespirations.Core;
using Homespirations.Core.DTOs;
using Homespirations.Core.Interfaces;
using Homespirations.Infrastructure.Identity;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async ([FromBody] RegisterRequest request, IUserService userService,
            UserManager<AppUser> userManager, IConfiguration config) =>
        {
            var (success, errors, user) = await userService.CreateUserAsync(request);
            if (!success)
                return Results.BadRequest(errors);


            // Generate Email Confirmation Token
            // var token = await userManager.GenerateEmailConfirmationTokenAsync(user!);
            // var encodedToken = Uri.EscapeDataString(token);

            // var confirmUrl = $"https://yourdomain.com/auth/confirm-email?email={user!.Email}&token={encodedToken}";

            // // Send Email via AWS SES
            // var sendRequest = new SendEmailRequest
            // {
            //     Source = "no-reply@yourdomain.com",
            //     Destination = new Destination { ToAddresses = new List<string> { user.Email! } },
            //     Message = new Message
            //     {
            //         Subject = new Content("Confirm Your Email"),
            //         Body = new Body
            //         {
            //             Html = new Content($"<p>Click <a href='{confirmUrl}'>here</a> to confirm your email.</p>")
            //         }
            //     }
            // };

            // await ses.SendEmailAsync(sendRequest);

            return Results.Ok("Registration successful! Please check your email to confirm your account.");
        });
    }
}
