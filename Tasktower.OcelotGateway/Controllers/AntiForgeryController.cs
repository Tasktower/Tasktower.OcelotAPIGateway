using Microsoft.AspNetCore.Antiforgery;
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
        
        [IgnoreAntiforgeryToken]
        public IActionResult Get()
        {
            return NoContent();
        }
    }
}