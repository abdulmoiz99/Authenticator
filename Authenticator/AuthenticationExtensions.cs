using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Authenticator.Model.Common;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Authenticator
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddGoogleAndGithubAuthentication(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<Services.AuthenticationService>(); // Ensure this line is present


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
                // Get the authentication settings from the DI container
                var serviceProvider = services.BuildServiceProvider(); // Create a service provider to resolve IOptions
                var settings = serviceProvider.GetRequiredService<IOptions<AuthenticationSettings>>().Value;

                options.ClientId = settings.Google.ClientId;
                options.ClientSecret = settings.Google.ClientSecret;
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
                    context.HandleResponse();
                };
            })
            .AddOAuth("GitHub", options =>
            {
                // Get the GitHub settings from the DI container
                var serviceProvider = services.BuildServiceProvider(); // Create a service provider to resolve IOptions
                var settings = serviceProvider.GetRequiredService<IOptions<AuthenticationSettings>>().Value;

                options.ClientId = settings.GitHub.ClientId;
                options.ClientSecret = settings.GitHub.ClientSecret;
                options.CallbackPath = new PathString("/api/authentication/github-response");

                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";

                options.Scope.Add("user:email");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, options.UserInformationEndpoint);
                        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var userJson = await response.Content.ReadAsStringAsync();
                        var user = System.Text.Json.JsonDocument.Parse(userJson);

                        context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.RootElement.GetString("id")));
                        context.Identity.AddClaim(new Claim(ClaimTypes.Name, user.RootElement.GetString("name")));
                        context.Identity.AddClaim(new Claim(ClaimTypes.Email, user.RootElement.GetString("blog")));
                    }
                };
            });
            return services;
        }
    }
}