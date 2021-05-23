using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasktower.OcelotGateway.Dtos;
using Tasktower.OcelotGateway.Security;

namespace Tasktower.OcelotGateway.Controllers
{
    [ApiController]
    [Route("auth-client")]
    public class AuthController : ControllerBase
    {
        [HttpGet("login")]
        public async Task Login([FromQuery]string returnUrl)
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
        
        [Authorize]
        [HttpGet("logout")]
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
        
        [HttpGet("tokens")]
        public async Task<TokensDto> GetTokens()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
    
                // if you need to check the Access Token expiration time, use this value
                // provided on the authorization response and stored.
                // do not attempt to inspect/decode the access token
                DateTime accessTokenExpiresAt = DateTime.Parse(
                    await HttpContext.GetTokenAsync("expires_at") ?? string.Empty, 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);
        
                string idToken = await HttpContext.GetTokenAsync("id_token");

                // Now you can use them. For more info on when and how to use the
                // Access Token and ID Token, see https://auth0.com/docs/tokens
                return new TokensDto(){IdToken = idToken, AccessToken = accessToken, 
                    AccessTokenExpirationDate = accessTokenExpiresAt};
            }
            return new TokensDto{};
        }
        
        [HttpGet("user")]
        public UserContext GetUser()
        {
            return UserContext.FromHttpContext(HttpContext);
        }
    }
    
    
}