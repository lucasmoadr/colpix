using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Colpix.Data.Models;
using Colpix.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Colpix.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly byte[] _keyBytes;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            _keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
        }

        public TokenResult CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id.ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_settings.TokenLifetimeMinutes);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenResult(tokenString, expires);
        }
    }
}