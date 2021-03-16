using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tasktower.OcelotAPIGateway.Middleware
{
    public class ClaimsToHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsToHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;
            context.Request.Headers["userid"] = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "";
            context.Request.Headers["name"] = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value ?? "";
            context.Request.Headers["email"] = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email")?.Value ?? "";
            context.Request.Headers["roles"] = user.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                .Select(c => c.Value)
                .Aggregate("", (current, next) => current + ',' + next);

            await _next(context);
        }
    }
}
