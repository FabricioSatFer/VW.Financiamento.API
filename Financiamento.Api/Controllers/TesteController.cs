using Microsoft.AspNetCore.Mvc;
using Financiamento.Domain.Repositories;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/teste")]
    public class TesteController : ControllerBase
    {
        private readonly Financiamento.Domain.Repositories.IContratosRepository _contratos;

        public TesteController(Financiamento.Domain.Repositories.IContratosRepository contratos)
        {
            _contratos = contratos;
        }

        [HttpGet("contratos")]
        public IActionResult GetContratos()
        {
            var all = _contratos.GetAll();
            return Ok(all);
        }
    }
}
