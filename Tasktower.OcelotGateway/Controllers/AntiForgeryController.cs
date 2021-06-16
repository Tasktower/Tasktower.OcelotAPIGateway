using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tasktower.OcelotGateway.Configuration.StartupExtensions;

namespace Tasktower.OcelotGateway.Controllers
{
    [ApiController]
    [Route("client/anti-forgery")]
    public class AntiForgeryController : ControllerBase
    {
        private readonly IAntiforgery _antiForgery;

        public AntiForgeryController(IAntiforgery antiForgery)
        {
            _antiForgery = antiForgery;
        }

        public IActionResult Get()
        {
            var tokens = _antiForgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append(SecurityConfig.XsrfRequestTokenCookieName, 
                tokens.RequestToken ?? string.Empty, 
                new CookieOptions
                {
                    HttpOnly = false
                });
            return NoContent();
        }
        
        // for testing antiforgery only
        [HttpPost("test")]
        public object PostTest()
        {
            return new
            {
                ok = "success"
            };
        }
    }
}