using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AuthController> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success && result.RequiresTwoFactor)
                return Ok(result); // Prompt for 2FA
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("2fa")]
        public async Task<IActionResult> TwoFactorLogin([FromBody] TwoFactorLoginDto dto)
        {
            var result = await _authService.TwoFactorLoginAsync(dto);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
