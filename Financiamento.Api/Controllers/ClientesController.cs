using Financiamento.Application.Interfaces;
using Financiamento.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/clientes/{cpfCnpj}/resumo")]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesService _clientesService;

        public ClientesController(IClientesService clientesService)
        {
            _clientesService = clientesService;
        }

        [HttpGet]
        public IActionResult Get(string cpfCnpj)
            => _clientesService.GetResumoCliente(cpfCnpj) != null
                ? Ok(_clientesService.GetResumoCliente(cpfCnpj))
                : NotFound();
    }
}
