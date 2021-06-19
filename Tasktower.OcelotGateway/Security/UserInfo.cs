using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Librame.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Json.Linq;

namespace Tasktower.OcelotGateway.Security
{
    public class UserInfo
    {
        public static async Task<UserInfo> FromHttpContext(HttpContext context, bool isWebApp)
        {
            var defaultUserInfo = new UserInfo
            {
                IsAuthenticated = false,
                UserId = "ANONYMOUS",
                Name = "ANONYMOUS",
                Permissions = new List<string>()
            };

            string accessTokenString; 
            if (isWebApp)
            {
                accessTokenString = context == null || (!context.User.Identity?.IsAuthenticated ?? false)? 
                    "" : await context.GetTokenAsync("access_token");
            }
            else
            {
                
                var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault() ?? "";
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
        
        private UserInfo() {}

        public bool IsAuthenticated { get; internal set; }

        public string UserId { get; internal set; }

        public string Name { get; internal set; }

        public IEnumerable<string> Permissions { get; internal set; }
    }
}
