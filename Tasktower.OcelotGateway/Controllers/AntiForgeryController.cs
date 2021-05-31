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
            var context = HttpContext;
            var tokens = _antiForgery.GetAndStoreTokens(context);
            context.Response.Cookies.Append(SecurityConfig.XsrfRequestTokenCookieName, tokens.RequestToken ?? string.Empty, 
                new CookieOptions
                {
                    HttpOnly = false
                });
            context.Response.Cookies.Append(SecurityConfig.XCsrfCookieName, tokens.CookieToken ?? string.Empty, 
                new CookieOptions
                {
                    HttpOnly = true
                });
            return NoContent();
        }
    }
}