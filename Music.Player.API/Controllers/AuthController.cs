using Amazon.Runtime.Internal;
using Entities.Auth;
using Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service;
using System.Threading.Tasks;

namespace Music.Player.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly ITokenService  _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginDTO request)
        {
            var user = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            JwtConfig config = new() { SecretKey = _configuration["Jwt:Key"], Audience = _configuration["Jwt:Audience"], Issuer = _configuration["Jwt:Issuer"], ExpiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]) };
            var token = _tokenService.GenerateToken(request.Username, config);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDTO request)
        {
            if (await _authService.UsernameExistsAsync(request.Username))
            {
                return BadRequest("Username already exists");
            }

            if (await _authService.EmailExistsAsync(request.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = "User" // Default role
            };

            var createdUser = await _authService.RegisterAsync(user, request.Password);
            return CreatedAtAction(nameof(Login), new { username = createdUser.Username }, createdUser);
        }
    }
}

