using AuthWebApi.Models;
using AuthWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AuthService _authService;
        private readonly IConfiguration _config;
        public AuthController(TokenService tokenService, IConfiguration config, AuthService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.RegisterAsync(model.Email, model.Password);
            if (!success)
                return BadRequest("Email already exists");

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.LoginAsync(model.Email, model.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var tokens = await _tokenService.GenerateTokens(user.Id.ToString(), user.Email);
            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var principal = await _tokenService.ValidateToken(request.AccessToken);
            if (principal == null || !await _tokenService.IsValidRefreshToken(request.RefreshToken))
                return Unauthorized("Invalid token");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            // Revoke old refresh token
            await _tokenService.RevokeRefreshToken(request.RefreshToken);

            var newTokens = await _tokenService.GenerateTokens(userId, email);
            return Ok(newTokens);
        }


        [HttpPost("revoke")]
        [Authorize] // Requires valid JWT
        public async Task<IActionResult> RevokeToken([FromBody] RevokeRequest request)
        {
            var success = await _tokenService.RevokeRefreshToken(request.RefreshToken);
            if (!success)
                return BadRequest("Invalid refresh token");

            return Ok(new { Message = "Token revoked successfully" });
        }
    }
}
