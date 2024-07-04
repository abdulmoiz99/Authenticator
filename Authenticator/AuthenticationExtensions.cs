using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Authenticator.Model.Common;
using System.Net;
using Microsoft.Extensions.Logging;
using Authenticator.Services;
using Microsoft.AspNetCore.Http;

namespace Authenticator
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services, string clientId, string clientSecret)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<GoogleAuthenticationService>(); // Ensure this line is present

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/authentication/google-login";
            })
            .AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.Events.OnRemoteFailure = async context =>
                {
                    var response = new AutoResponseDto<string>
                    {
                        Success = false,
                        Message = "Authentication failed",
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    response.AddError(context.Failure.Message);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(response);
                    context.HandleResponse(); // Suppress the exception
                };
            });

            return services;
        }
    }
}
