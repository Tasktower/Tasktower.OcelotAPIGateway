using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Librame.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Tasktower.OcelotGateway.Configuration.StartupExtensions;
using Tasktower.OcelotGateway.Dtos;
using Tasktower.OcelotGateway.Security;

namespace Tasktower.OcelotGateway.Controllers
{
    [ApiController]
    [Route("client/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task Login([FromQuery]string returnUrl)
        {
            if (IsWebApp)
            {
                await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties()
                {
                    RedirectUri = returnUrl
                });
            }
        }
        
        [HttpPost("logout")]
        public async Task Logout([FromQuery]string returnUrl)
        {
            if (HttpContext.User.Identity != null && IsWebApp && HttpContext.User.Identity.IsAuthenticated)
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
        }
        
        [HttpGet("user")]
        public async Task<UserInfo> GetUser()
        {
            var defaultUserInfo = new UserInfo
            {
                IsAuthenticated = false,
                UserId = "ANONYMOUS",
                Name = "ANONYMOUS",
                Permissions = new List<string>()
            };

            string accessTokenString; 
            if (IsWebApp)
            {
                accessTokenString = HttpContext == null || (!HttpContext.User.Identity?.IsAuthenticated ?? false)? 
                    "" : await HttpContext.GetTokenAsync("access_token");
            }
            else
            {
                var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault() ?? "";
                accessTokenString = authorizationHeader.StartsWith("Bearer ")? authorizationHeader[7..]: "";
            }
            
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(accessTokenString)) return defaultUserInfo;

            var securityToken = handler.ReadJwtToken(accessTokenString);
            IDictionary<string, IEnumerable<Claim>> claims = securityToken.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key,g => g.AsEnumerable());
            return new UserInfo
            {
                IsAuthenticated = securityToken.IsNotNull(),
                UserId = securityToken.Subject,
                Name = claims[ClaimTypes.Name].First().Value,
                Permissions = claims["permissions"].Select(c => c.Value)
            };
        }

        private bool IsWebApp => _configuration.GetValue("GatewayInfo:WebApp", true);
    }
    
    
}