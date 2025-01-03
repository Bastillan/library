using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReactLibrary.Server.Data;
using ReactLibrary.Server.Models;
using ReactLibrary.Server.Models.Api;

namespace ReactLibrary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (User.Identity?.Name != user.UserName)
            {
                return Unauthorized();
            }
            var userDTO = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
            return Ok(userDTO);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(request.UserName!);
            if (user == null)
            {
                return BadRequest("Invalid login attempt");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
            if (!isPasswordValid)
            {
                return BadRequest("Invalid login attempt");
            }

            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);
            if (userInDb == null)
            {
                return Unauthorized();
            }

            var accessToken = await GenerateToken(userInDb);
            var refreshToken = GenerateRefreshToken();
            userInDb.RefreshToken = refreshToken;
            userInDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(userInDb);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                UserId = userInDb.Id,
                AuthToken = accessToken,
                RefreshToken = refreshToken
            });

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(user, request.Password!);

            if (result.Succeeded)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);
                if (userInDb == null)
                {
                    return Unauthorized();
                }

                var accessToken = await GenerateToken(userInDb);

                await _userManager.AddToRoleAsync(user, "Reader");
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { userInDb.Id }, new AuthResponse
                {
                    UserId = userInDb.Id,
                    AuthToken = accessToken,
                    RefreshToken = string.Empty
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(String id)
        {
            //if (User.IsInRole("Librarian"))
            //return Ok(User.Identity.IsAuthenticated);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Id != id)
            {
                return BadRequest("Can't delete another user");
            }

            if (User.IsInRole("Librarian"))
            {
                return BadRequest("Can't delete Librarian");
            }

            if (_context.Checkout.Any(c => c.EndTime == null && c.UserName == User.Identity.Name))
            {
                return BadRequest("You have some books still not returned");
            }
            
            var result = await _userManager.DeleteAsync(user);
            
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest tokenRequest)
        {
            if (tokenRequest is null)
            {
                return BadRequest("Invalid client request");
            }

            var principal = GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var username = principal.Identity?.Name;
            var user = await _userManager.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = await GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshExpirationInDays"));

            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponse
            {
                AuthToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id
            });
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var handler = new JwtSecurityTokenHandler();

            var issuer = _configuration.GetSection("JWT")["ValidIssuer"];
            var audience = _configuration.GetSection("JWT")["ValidAudience"];
            var claims = await CreateClaims(user);
            var expiration = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWT:ExpirationInMinutes"));
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
                new Claim(ClaimTypes.Name, user.UserName!),

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

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
