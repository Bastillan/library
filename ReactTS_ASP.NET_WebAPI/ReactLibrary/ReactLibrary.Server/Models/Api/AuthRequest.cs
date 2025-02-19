using System.ComponentModel.DataAnnotations;

namespace ReactLibrary.Server.Models.Api
{
    public class AuthRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
