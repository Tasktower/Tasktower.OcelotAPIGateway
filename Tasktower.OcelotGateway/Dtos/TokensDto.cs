using System;

namespace Tasktower.OcelotGateway.Dtos
{
    public class TokensDto
    {
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public DateTime AccessTokenExpirationDate { get; set; }
    }
}