using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduTakip.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EduTakip.Services
{
    public interface IJwtService
    {
        string CreateToken(ItemKullanici user);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtOptions _options;
        private readonly SymmetricSecurityKey _securityKey;

        public JwtService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        }

        public string CreateToken(ItemKullanici user)
        {
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EPosta),
                new Claim("name", $"{user.Adi} {user.Soyadi}".Trim()),
                new Claim("role", user.KullaniciTip?.Adi ?? string.Empty)
            };

            var expires = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

