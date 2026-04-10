using Financiamento.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        private readonly IContratosRepository _contratos;

        public TesteController(IContratosRepository contratos)
        {
            _contratos = contratos;
        }

        [HttpGet("contratos")]
        public async Task<IActionResult> GetContratos()
        {
            var all = await _contratos.GetAll();
            return Ok(all);
        }
    }
}
