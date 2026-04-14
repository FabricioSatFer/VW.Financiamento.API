using Microsoft.AspNetCore.Mvc;
using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/contratos")]
    [Authorize]
    public class ContratosController : ControllerBase
    {
        private readonly IContratosServices _contratosService;

        public ContratosController(IContratosServices contratosService)
        {
            _contratosService = contratosService;
        }

        [HttpPost]
        public async Task<ActionResult<ContratoDto>> Create([FromBody] ContratoCreateDto dto)
        {
            var result = await _contratosService.Create(dto);
            if (!result.Success)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = string.Join(";", result.Errors)
                });
            }

            if (result.Value is null)
            {
                return Problem(detail: "Contrato criado mas não foi possível retornar representação.", statusCode: 500);
            }

            return CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> List()
            => Ok(await _contratosService.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
            => await _contratosService.Get(id) is ContratoDto c ? Ok(c) : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _contratosService.Delete(id) ? NoContent() : NotFound();
    }
}

