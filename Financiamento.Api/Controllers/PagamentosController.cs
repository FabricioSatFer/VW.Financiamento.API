using Microsoft.AspNetCore.Mvc;
using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/contratos/{contratoId}/pagamentos")]
    [Authorize]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentosServices _pagamentosService;


        public PagamentosController(IPagamentosServices pagamentosService)
        {
            _pagamentosService = pagamentosService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid contratoId, [FromBody] PagamentoCreateDto dto)
            => await _pagamentosService.RegistrarPagamento(contratoId, dto) != null 
                ? CreatedAtAction(nameof(GetAll), new { contratoId }, dto) 
                : NotFound();

        [HttpGet]
        public IActionResult GetAll(Guid contratoId)
            => _pagamentosService.GetByContrato(contratoId) != null 
                ? Ok(_pagamentosService.GetByContrato(contratoId)) 
                : NotFound();
    }
}
