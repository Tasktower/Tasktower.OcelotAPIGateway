using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Tasktower.OcelotGateway.Security.Middlewares
{
    public class WebAppCookieToOpenIdTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public WebAppCookieToOpenIdTokenMiddleware(RequestDelegate next, ILogger<WebAppCookieToOpenIdTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var accessToken = await context.GetTokenAsync("access_token");
                context.Request.Headers["Authorization"] = $"Bearer {accessToken ?? ""}";
            }
            else
            {
                context.Request.Headers["Authorization"] = "";
            }
            await _next(context);
        }
    }
}