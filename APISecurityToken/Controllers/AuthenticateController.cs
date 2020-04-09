using APISecurityToken.Interfaces;
using APISecurityToken.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace APISecurityToken.Controllers
{
    [Route("v1/authenticate")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        readonly ITokenService tokenService;

        public AuthenticateController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("gettoken")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]AuthenticationData model)
        {
            var resultToken = await tokenService.GenerateToken(model);
            if (!resultToken.PINIsValid)
                return BadRequest(resultToken);
            else
                return Ok(resultToken);
        }
    }
}