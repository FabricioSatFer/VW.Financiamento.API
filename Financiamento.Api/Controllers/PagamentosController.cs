using System;
using Microsoft.AspNetCore.Mvc;
using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;

namespace Financiamento.Api.Controllers
{
    [ApiController]
    [Route("api/contratos/{contratoId}/pagamentos")]
    public class PagamentosController : ControllerBase
    {
        private readonly Financiamento.Domain.Repositories.IContratosRepository _contratoRepo;
        private readonly Financiamento.Domain.Repositories.IPagamentosRepository _pagamentoRepo;

        public PagamentosController(Financiamento.Domain.Repositories.IContratosRepository contratoRepo, Financiamento.Domain.Repositories.IPagamentosRepository pagamentoRepo)
        {
            _contratoRepo = contratoRepo;
            _pagamentoRepo = pagamentoRepo;
        }

        [HttpPost]
        public IActionResult Create(Guid contratoId, [FromBody] PagamentoCreateDto dto)
        {
            var contrato = _contratoRepo.Get(contratoId);
            if (contrato == null) return NotFound();

            var dataVenc = contrato.DataVencimentoPrimeiraParcela.AddMonths(dto.ParcelaNumero - 1);
            var status = dto.DataPagamento.Date > dataVenc.Date ? StatusPagamento.EmAtraso : (dto.DataPagamento.Date < dataVenc.Date ? StatusPagamento.Antecipado : StatusPagamento.EmDia);

            var pagamento = new Pagamento
            {
                ContratoId = contratoId,
                ParcelaNumero = dto.ParcelaNumero,
                ValorPago = dto.ValorPago,
                DataPagamento = dto.DataPagamento,
                DataVencimento = dataVenc,
                Status = status
            };

            _pagamentoRepo.Add(pagamento);
            contrato.Pagamentos.Add(pagamento);
            _contratoRepo.Update(contrato);

            return CreatedAtAction(nameof(GetAll), new { contratoId }, pagamento);
        }

        [HttpGet]
        public IActionResult GetAll(Guid contratoId)
        {
            var contrato = _contratoRepo.Get(contratoId);
            if (contrato == null) return NotFound();
            return Ok(_pagamentoRepo.GetByContrato(contratoId));
        }
    }
}
