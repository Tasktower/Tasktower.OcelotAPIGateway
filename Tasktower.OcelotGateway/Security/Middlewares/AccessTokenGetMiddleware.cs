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
    public class AccessTokenGetMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public AccessTokenGetMiddleware(RequestDelegate next, ILogger<AccessTokenGetMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //If a user is authenticated, it means their cookies are correct so a bearer token is set for them.
            //In other cases, if you are just using a bearer token, it will just be passed down the pipeline.
            //Your bearer token will be overwritten however if you are using browser cookies and they are valid.
            //So bearer authentication is to be used if and only if you are not using a web app (i.e. desktop or mobile)
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var accessToken = await context.GetTokenAsync("access_token");
                context.Request.Headers["Authorization"] = $"Bearer {accessToken ?? ""}";
            }
            await _next(context);
        }
    }
}