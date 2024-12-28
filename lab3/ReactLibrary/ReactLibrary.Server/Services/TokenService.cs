using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

using ReactLibrary.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace ReactLibrary.Server.Services
{
    public class TokenService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public TokenService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var handler = new JwtSecurityTokenHandler();

            var issuer = _configuration.GetSection("JWT")["ValidIssuer"];
            var audience = _configuration.GetSection("JWT")["ValidAudience"];
            var claims = await CreateClaims(user);
            var expiration = DateTime.UtcNow.AddMinutes(15);
            var credentials = CreateSigningCredentials();

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiration,
                signingCredentials: credentials
                );

            return handler.WriteToken(token);
        }

        private async Task<List<Claim>> CreateClaims(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),

            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = _configuration.GetSection("JWT")["Secret"];

            return new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricSecurityKey)), SecurityAlgorithms.HmacSha256);
        }
    }
}
