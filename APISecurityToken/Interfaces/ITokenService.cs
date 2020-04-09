using APISecurityToken.Models;
using System.Threading.Tasks;

namespace APISecurityToken.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticationResult> GenerateToken(AuthenticationData authenticationData);
    }
}
