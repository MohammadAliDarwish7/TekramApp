using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TekramApp.Models;

namespace TekramApp.Helpers
{
    public class JWTHelper
    {
        public class JwtSettings
        {
            public string Secret { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public int ExpiryMinutes { get; set; }
        }

        public interface IJwtTokenService
        {
            string GenerateToken(User user);
            (string Token, DateTime Expires) GenerateToken(Customer customer, TimeSpan duration);

        }

        public class JwtTokenService : IJwtTokenService
        {
            private readonly JwtSettings _jwtSettings;

            public JwtTokenService(IOptions<JwtSettings> jwtSettings)
            {
                _jwtSettings = jwtSettings.Value;
            }

            // Generate token for User
            public string GenerateToken(User user)
            {
                var permissions = user.Role?.RolePermissions.Select(rp => rp.Permission.Name).ToList()
                                  ?? new List<string>();

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

                // Add permission claims
                claims.AddRange(permissions.Select(p => new Claim("permission", p)));

                var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

                return CreateToken(claims, expires);
            }

            // Generate token for Customer with custom duration
            public (string Token, DateTime Expires) GenerateToken(Customer customer, TimeSpan duration)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, customer.Username),
                    new Claim(JwtRegisteredClaimNames.Email, customer.Email)
                };

                var expires = DateTime.UtcNow.Add(duration);
                var token = CreateToken(claims, expires);
                return (token, expires);
            }

            // Private helper method to create token
            private string CreateToken(IEnumerable<Claim> claims, DateTime expires)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
}