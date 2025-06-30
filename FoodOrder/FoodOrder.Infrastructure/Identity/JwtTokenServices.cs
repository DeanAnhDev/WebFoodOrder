using FoodOrder.Application.Services;
using FoodOrder.Application.Services.Auth;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Infrastructure.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrder.Infrastructure.Identity
{
    public class JwtTokenServices : IJwtTokenServices
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly IRedisService _redisService;

        public JwtTokenServices(
              UserManager<AppUser> userManager,
              IOptions<JwtOptions> jwtOptions,
              IRedisService redisService)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value; 
            _redisService = redisService;
        }
        public async Task<string> GenerateTokenAsync(AppUser user, DateTime expiryTime, bool isAccessToken)
        {
            // Get the user claims.
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

            var securityStamp = await _userManager.GetSecurityStampAsync(user);

            var jti = Guid.NewGuid().ToString();

            // Store the jti in Redis with the user ID as the key.
            if (isAccessToken)
            {
                await _redisService.SetJtiAsync(user.Id.ToString(), jti, (expiryTime - DateTime.Now).TotalSeconds);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email,user.Email!),
                new(JwtRegisteredClaimNames.Jti, jti),
                new("AspNet.Identity.SecurityStamp",securityStamp),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiryTime,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            return await GenerateTokenAsync(user, DateTime.Now.AddMinutes(_jwtOptions.ExpiryMinutes), true);
        }

        public async Task<string> GenerateRefreshTokenAsync(AppUser user)
        {
            // Tạo chuỗi ngẫu nhiên làm refresh token
            var refreshToken = Guid.NewGuid().ToString("N");

            var success = await _redisService.SaveRefreshTokenAsync(
                user.Id.ToString(),
                refreshToken,
                7 
            );

            if (!success)
            {
                throw new Exception("Không thể lưu refresh token vào Redis");
            }

            return refreshToken;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ValidateLifetime = false, // Không check thời gian ở đây, vì token có thể đã hết hạn
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }


    }
}
