using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/contratos")]
    public class ContratosController : ControllerBase
    {
        private readonly IContratosServices _contratosService;

        public ContratosController(IContratosServices contratosService)
        {
            _contratosService = contratosService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContratoCreateDto dto)
            => await _contratosService.Create(dto) ? Ok() : BadRequest();

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

