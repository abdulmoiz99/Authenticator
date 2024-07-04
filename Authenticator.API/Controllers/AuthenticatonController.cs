using Authenticator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatonController : ControllerBase
    {
        private readonly GoogleAuthenticationService _authenticationService;

        public AuthenticatonController(GoogleAuthenticationService authenticationService)
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
    }
}
