using System;

namespace Tasktower.OcelotGateway.Dtos
{
    public class TokensDto
    {
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }
        
        public string IdToken { get; set; }
    }
}