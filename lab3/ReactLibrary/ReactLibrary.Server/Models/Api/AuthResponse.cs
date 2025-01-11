using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models.Api
{
    public class AuthResponse
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
