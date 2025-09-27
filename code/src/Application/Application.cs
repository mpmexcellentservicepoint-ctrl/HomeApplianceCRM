namespace Application {
    // Application services, interfaces, and DTOs will be defined here.

    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task<AuthResultDto> TwoFactorLoginAsync(TwoFactorLoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task<AuthResultDto> LogoutAsync(LogoutRequestDto dto);
    }
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class TwoFactorLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string TwoFactorCode { get; set; } = string.Empty;
        public string TwoFactorProvider { get; set; } = string.Empty;
    }

    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; } = new List<string>();
        public bool RequiresTwoFactor { get; set; }
    }
}
