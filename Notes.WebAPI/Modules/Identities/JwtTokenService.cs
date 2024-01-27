using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Notes.Common.Security.Token;
using Notes.Services.Identities.Services;

namespace Notes.WebAPI.Modules.Identities
{
    public class JwtTokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _securityKey;
        
        public JwtTokenService(
            string securityKey)
        {
            _securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
        }
        
        public string IssueToken(
            TokenType tokenType,
            Dictionary<string, string> payload)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            payload.Add(nameof(TokenType), $"{tokenType}");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new List<Claim>(payload.Select(o => new Claim(o.Key, o.Value)))),
                Expires = DateTime.Now,
                SigningCredentials = new SigningCredentials(
                    _securityKey,
                    SecurityAlgorithms.HmacSha512Signature)
            };

            tokenDescriptor.Expires += tokenType switch
            {
                TokenType.AuthorizationCode => TimeSpan.FromMinutes(1),
                TokenType.AccessToken => TimeSpan.FromHours(8),
                TokenType.RefreshToken => TimeSpan.FromDays(1),
                _ => throw new ArgumentOutOfRangeException()
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}