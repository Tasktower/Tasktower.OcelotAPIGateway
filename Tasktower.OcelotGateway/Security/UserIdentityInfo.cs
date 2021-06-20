namespace Tasktower.OcelotGateway.Security
{
    public class UserIdentityInfo
    {
        public bool IsAuthenticated { get; init; }
        
        public bool EmailVerified { get; set; }

        public string UserId { get; init; }

        public string Name { get; init; }
        
        public string Email { get; init; }
        
        public string Nickname { get; init; }
        
        public string Picture { get; set; }
    }
}