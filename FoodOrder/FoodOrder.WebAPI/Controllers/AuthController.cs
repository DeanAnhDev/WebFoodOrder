using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.DTOs.Authentication;
using FoodOrder.Application.Services.Auth;
using FoodOrder.Infrastructure.Identity;
using FoodOrder.Infrastructure.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var result = await _authService.RegisterAsync(registerUser);

            if (!result.Status)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
        {
            var response = await _authService.ConfirmEmailAsync(token, email);

            if (!response.Status)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Status)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var result = await _authService.LogoutAsync(request.AccessToken, request.RefreshToken);
            return result ? Ok(new { message = "Đã đăng xuất" }) : BadRequest(new { message = "Đăng xuất thất bại" });
        }



    }
}
