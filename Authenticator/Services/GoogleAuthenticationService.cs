using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Authenticator.Model;
using Authenticator.Model.Common;
using System.Security.Claims;
using System.Net;

namespace Authenticator.Services
{
    public class GoogleAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult GoogleLogin(string redirectUri)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<object?> GoogleResponse()
        {
            var response = new AutoResponseDto<UserInfoDto>();

            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext == null)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.AddError("HttpContext is null");
                return response;
            }

            var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                response.Success = false;
                response.StatusCode = HttpStatusCode.Unauthorized;
                response.AddError("Authentication failed or principal is null");
                return response;
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var userInfo = new UserInfoDto
            {
                Id = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                GivenName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                Surname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                Email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };

            response.Result = userInfo;
            return response;
        }
    }
}
