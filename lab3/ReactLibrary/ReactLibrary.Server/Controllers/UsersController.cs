﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReactLibrary.Server.Data;
using ReactLibrary.Server.Models;

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
                return BadRequest("Bad credentials");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
            if (!isPasswordValid)
            {
                return BadRequest("Bad credentials");
            }

            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);
            if (userInDb == null)
            {
                return Unauthorized();
            }

            var accessToken = await GenerateToken(userInDb);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                UserId = userInDb.Id,
                AuthToken = accessToken,
                RefreshToken = string.Empty
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
                request.Password = "";
                await _userManager.AddToRoleAsync(user, "Reader");
                return CreatedAtAction(nameof(Register), new { username = request.UserName, role = "Reader" }, request);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        private async Task<string> GenerateToken(ApplicationUser user)
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

    }
}
