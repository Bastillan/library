namespace ReactLibrary.Server.Models
{
    public class AuthResponse
    {
        public string UserId { get; set; }
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
