using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationServices _authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            var result = await _authenticationServices.Autenticar(req);
            if (result == null)
                return Unauthorized(new { message = "Usuário ou senha incorretos." });

            return Ok(result);
        }
    }
}