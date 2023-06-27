using AMS.Interfaces;
using AMS.Models;
using AMS.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMS.Providers
{
    public class JwtProvider : IJwtProvider
    {
        private readonly Jwt jwt;

        public JwtProvider(IConfiguration Config)
        {
            jwt = Config.GetSection("Jwt").Get<Jwt>();
        }

        public Jwt Jwt => jwt;

        public string GenerateToken(ApplicationUser user)
        {

            var claims = new List<Claim>()
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.PrimaryGroupSid, user.Organization?.Id.ToString()),
        };

            foreach (Role role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Id));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
