using Financiamento.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/clientes/{cpfCnpj}/resumo")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesServices _clientesService;

        public ClientesController(IClientesServices clientesService)
        {
            _clientesService = clientesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string cpfCnpj)
        {
            var resumo = await _clientesService.GetResumoCliente(cpfCnpj);
            return resumo != null ? Ok(resumo) : NotFound();
        }
    }
}
