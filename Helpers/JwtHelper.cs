using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ChhayaNirh.Helpers
{
    public static class JwtHelper
    {
        private static readonly string SecretKey = ConfigurationManager.AppSettings["JwtSecretKey"] ?? "your-super-secret-key-here-make-it-long-and-complex";
        private static readonly string Issuer = "ChhayaNirh";

        public static string GenerateToken(int userId, string userName, string userType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", userId.ToString()),
                    new Claim("UserName", userName),
                    new Claim("UserType", userType)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = Issuer,
                Audience = Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Issuer,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Create ClaimsIdentity with "UserId" as first claim
                var claimsIdentity = new ClaimsIdentity("jwt");
                claimsIdentity.AddClaim(jwtToken.Claims.First(c => c.Type == "UserId"));
                foreach (var claim in jwtToken.Claims.Where(c => c.Type != "UserId"))
                    claimsIdentity.AddClaim(claim);

                return new ClaimsPrincipal(claimsIdentity);
            }
            catch
            {
                return null;
            }
        }

    }
}