using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasktower.OcelotGateway.Security;

namespace Tasktower.OcelotGateway.Controllers
{
    [ApiController]
    [Route("client/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task Login([FromQuery]string returnUrl)
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
        
        [Authorize]
        [HttpPost("logout")]
        public async Task Logout([FromQuery]string returnUrl)
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be added to the
                // **Allowed Logout URLs** settings for the app.
                RedirectUri = returnUrl
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet("user")]
        public UserContext GetUser()
        {
            return UserContext.FromHttpContext(HttpContext);
        }
    }
    
    
}