using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using ReactLibrary.Server.Data;
using ReactLibrary.Server.Models;
using ReactLibrary.Server.Models.Api;
using ReactLibrary.Server.Services;

namespace ReactLibrary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            TokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
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

            var accessToken = await _tokenService.GenerateTokenAsync(userInDb);
            var refreshToken = _tokenService.GenerateRefreshToken();
            userInDb.RefreshToken = refreshToken;
            userInDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshExpirationInDays"));
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

                var accessToken = await _tokenService.GenerateTokenAsync(userInDb);

                await _userManager.AddToRoleAsync(user, "Reader");

                var refreshToken = _tokenService.GenerateRefreshToken();
                userInDb.RefreshToken = refreshToken;
                userInDb.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshExpirationInDays"));
                await _userManager.UpdateAsync(userInDb);

                return CreatedAtAction(nameof(GetUser), new { userInDb.Id }, new AuthResponse
                {
                    UserId = userInDb.Id,
                    AuthToken = accessToken,
                    RefreshToken = refreshToken
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

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
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

            var newAccessToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

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
    }
}
