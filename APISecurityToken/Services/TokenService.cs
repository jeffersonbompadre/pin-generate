using APISecurityToken.Interfaces;
using APISecurityToken.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PINDomain.Interfaces;
using PINDomain.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APISecurityToken.Services
{
    public class TokenService : ITokenService
    {
        readonly IPINGenerator pinGenerator;
        readonly IConfiguration configuration;

        public TokenService(IPINGenerator pinGenerator, IConfiguration configuration)
        {
            this.pinGenerator = pinGenerator;
            this.configuration = configuration;
        }

        public async Task<AuthenticationResult> GenerateToken(AuthenticationData authenticationData)
        {
            return await Task.Run(() =>
            {
                if (!pinGenerator.PINIsValid(authenticationData.PINData))
                    return new AuthenticationResult(
                        string.Empty,
                        false,
                        "PIN invalid",
                        authenticationData
                    );
                else
                {
                    var tokenExpires = int.Parse(configuration.GetSection("Parameters:TimeTokenExpires").Value);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(Settings.Secret);
                    var claims = new List<Claim>() { new Claim(ClaimTypes.Name, authenticationData.UserName) };
                    authenticationData.Roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddHours(tokenExpires),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return new AuthenticationResult(
                        tokenHandler.WriteToken(token),
                        true,
                        string.Empty,
                        authenticationData
                    );
                }
            });
        }
    }
}
