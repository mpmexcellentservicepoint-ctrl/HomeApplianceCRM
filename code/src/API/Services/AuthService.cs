
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration config, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _db = db;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var user = new AppUser { UserName = dto.Email, Email = dto.Email, Role = dto.Role };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            await _userManager.AddToRoleAsync(user, dto.Role);
            return new AuthResultDto { Success = true };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials" } };
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (result.RequiresTwoFactor)
                return new AuthResultDto { Success = false, RequiresTwoFactor = true };
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials" } };
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var refreshToken = await IssueRefreshTokenAsync(user);
            return new AuthResultDto { Success = true, Token = token, RefreshToken = refreshToken };
        }

        public async Task<AuthResultDto> TwoFactorLoginAsync(TwoFactorLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials" } };
            var result = await _signInManager.TwoFactorSignInAsync(dto.TwoFactorProvider, dto.TwoFactorCode, false, false);
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid 2FA code" } };
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var refreshToken = await IssueRefreshTokenAsync(user);
            return new AuthResultDto { Success = true, Token = token, RefreshToken = refreshToken };
        }
        public async Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid user" } };
            var refreshToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == dto.RefreshToken && rt.UserId == user.Id);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.ExpiresAt < DateTime.UtcNow)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid or expired refresh token" } };
            refreshToken.IsUsed = true;
            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var newRefreshToken = await IssueRefreshTokenAsync(user);
            return new AuthResultDto { Success = true, Token = token, RefreshToken = newRefreshToken };
        }

        public async Task<AuthResultDto> LogoutAsync(LogoutRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid user" } };
            var refreshToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == dto.RefreshToken && rt.UserId == user.Id);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                _db.RefreshTokens.Update(refreshToken);
                await _db.SaveChangesAsync();
            }
            return new AuthResultDto { Success = true };
        }

        private async Task<string> IssueRefreshTokenAsync(AppUser user)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                IsUsed = false,
                CreatedBy = user.Email ?? "system",
                CreatedAt = DateTime.UtcNow
            };
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();
            return token;
        }

        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new System.Security.Claims.Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };
            claims.AddRange(roles.Select(r => new System.Security.Claims.Claim(ClaimTypes.Role, r)));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "THIS IS A VERY SECURE KEY"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "HomeApplianceCRM",
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
