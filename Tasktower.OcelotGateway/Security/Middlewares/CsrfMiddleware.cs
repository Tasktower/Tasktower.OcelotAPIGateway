using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Tasktower.OcelotGateway.Configuration.StartupExtensions;

namespace Tasktower.OcelotGateway.Security.Middlewares
{
    public class CsrfMiddleware
    {
        private static readonly IEnumerable<string> SafeHttpMethods = new HashSet<string>()
        {
            HttpMethods.Get, HttpMethods.Options, HttpMethods.Head, HttpMethods.Trace
        };
            
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiForgery;

        public CsrfMiddleware(RequestDelegate next, IAntiforgery antiForgery)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _antiForgery = antiForgery;
        }

        public async Task Invoke(HttpContext context)
        {

            if (!SafeHttpMethods.Contains(context.Request.Method))
            {
                await _antiForgery.ValidateRequestAsync(context);
            }

            await _next.Invoke(context);
        }
    }
}