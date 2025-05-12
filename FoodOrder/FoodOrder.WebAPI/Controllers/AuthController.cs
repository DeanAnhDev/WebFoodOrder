using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.DTOs.Authentication;
using FoodOrder.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;


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


    }
}
