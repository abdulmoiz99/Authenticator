using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Authenticator.Model.Common;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Authenticator
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services, string clientId, string clientSecret)
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
            })
           .AddOAuth("GitHub", options =>
           {
               options.ClientId = "Ov23lipHpq3nb45mmJek"; // Use GitHub ClientId
               options.ClientSecret = "1b1d7278907a1fb9e714728e0ef8a44d7de0f85b"; // Use GitHub ClientSecret
               options.CallbackPath = new PathString("/api/authentication/github-response");

               // GitHub OAuth endpoints
               options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
               options.TokenEndpoint = "https://github.com/login/oauth/access_token";
               options.UserInformationEndpoint = "https://api.github.com/user";

               options.Scope.Add("user:email");

               options.Events = new OAuthEvents
               {
                   OnCreatingTicket = async context =>
                   {
                       // Make a request to the GitHub user endpoint to retrieve additional user data
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
