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
    public class UserAccessInfo
    {
        public bool IsAuthenticated { get; init; }

        public string UserId { get; init; }

        public string Name { get; init; }

        public IEnumerable<string> Permissions { get; init; }
    }
}
