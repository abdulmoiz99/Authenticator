using Authenticator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatonController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticatonController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
     
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUri = Url.Action("GoogleResponse");
            return _authenticationService.GoogleLogin(redirectUri);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var claims = await _authenticationService.GoogleResponse();
            return new JsonResult(claims);
        }
        [HttpGet("github-login")]
        public IActionResult GithbLogin()
        {
            var redirectUri = Url.Action("GithbResponse");
            return _authenticationService.GithubLogin(redirectUri);
        }
        [HttpGet("github-response")]
        public async Task<IActionResult> GithbResponse()
        {
            var claims = await _authenticationService.GitHubResponse();
            return new JsonResult(claims);
        }
    }
}
