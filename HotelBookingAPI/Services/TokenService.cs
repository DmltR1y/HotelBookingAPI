using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace HotelBookingAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, List<string> roles);
    }

    public class TokenService : ITokenService
    {
        private IConfiguration _config;
        private UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public string CreateToken(ApplicationUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Email, user.Email!),
                new (ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            };

            foreach (var role in roles)
            {
                claims.Add(new (ClaimTypes.Role, role));
            }

            var jwtKey = _config["Jwt:Key"];
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(8),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}