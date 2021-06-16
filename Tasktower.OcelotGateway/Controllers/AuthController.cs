using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasktower.OcelotGateway.Configuration.StartupExtensions;
using Tasktower.OcelotGateway.Dtos;
using Tasktower.OcelotGateway.Security;

namespace Tasktower.OcelotGateway.Controllers
{
    [ApiController]
    [Route("client/auth")]
    public class AuthController : ControllerBase
    {

        // [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task Login([FromQuery]string returnUrl)
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
        
        // [ValidateAntiForgeryToken]
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

        // [IgnoreAntiforgeryToken]
        // [HttpGet("tokens")]
        // public async ValueTask<TokensDto> Tokens()
        // {
        //     if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        //     {
        //         var accessTokenTask = HttpContext.GetTokenAsync("access_token");
        //         var accessTokenExpiresAtTask = HttpContext.GetTokenAsync("expires_at");
        //         var idTokenTask = HttpContext.GetTokenAsync("id_token");
        //         return new TokensDto
        //         {
        //             AccessToken = await accessTokenTask,
        //             AccessTokenExpiration = DateTime.Parse(
        //                 await accessTokenExpiresAtTask ?? string.Empty, 
        //                 CultureInfo.InvariantCulture,
        //                 DateTimeStyles.RoundtripKind),
        //             IdToken = await idTokenTask
        //         };
        //     }
        //
        //     return new TokensDto();
        // }
        
        // [IgnoreAntiforgeryToken]
        [HttpGet("user")]
        public UserContext GetUser()
        {
            return UserContext.FromHttpContext(HttpContext);
        }
    }
    
    
}