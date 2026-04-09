using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;
using Financiamento.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financiamento.Application.Services
{
    public class PagamentosServices : IPagamentosServices
    {
        private readonly IPagamentosRepository _pagamentosRepository;
        private readonly IContratosRepository _contratosRepository;

        public PagamentosServices(IPagamentosRepository pagamentosRepository, IContratosRepository contratosRepository)
        {
            _pagamentosRepository = pagamentosRepository;
            _contratosRepository = contratosRepository;
        }

        public Task<PagamentoDto> RegistrarPagamento(Guid contratoId, PagamentoCreateDto input)
        {
            var contrato = _contratosRepository.Get(contratoId);
            if (contrato == null) throw new ArgumentException("Contrato não encontrado", nameof(contratoId));

            var parcelaVencimento = contrato.DataVencimentoPrimeiraParcela.AddMonths(input.ParcelaNumero - 1);
            var status = input.DataPagamento.Date == parcelaVencimento.Date ? (int)StatusPagamento.EmDia
                        : input.DataPagamento.Date > parcelaVencimento.Date ? (int)StatusPagamento.EmAtraso
                        : (int)StatusPagamento.Antecipado;

            var pagamento = new Pagamento
            {
                ContratoId = contratoId,
                ParcelaNumero = input.ParcelaNumero,
                ValorPago = input.ValorPago,
                DataPagamento = input.DataPagamento.ToUniversalTime(),
                DataVencimento = parcelaVencimento,
                Status = status
            };

            var created = _pagamentosRepository.Add(pagamento);

            var dto = new PagamentoDto
            {
                Id = created.Id,
                ContratoId = created.ContratoId,
                ParcelaNumero = created.ParcelaNumero,
                ValorPago = created.ValorPago,
                DataPagamento = input.DataPagamento.ToUniversalTime(), 
                DataVencimento = created.DataVencimento.ToUniversalTime(), 
                Status = created.Status
            };

            return Task.FromResult(dto);
        }

        public Task<IEnumerable<PagamentoDto>> GetByContrato(Guid contratoId)
        {
            var pagamentos = _pagamentosRepository.GetByContrato(contratoId)
                .Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    ContratoId = p.ContratoId,
                    ParcelaNumero = p.ParcelaNumero,
                    ValorPago = p.ValorPago,
                    DataPagamento = p.DataPagamento,
                    DataVencimento = p.DataVencimento,
                    Status = p.Status
                });

            return Task.FromResult(pagamentos);
        }

        public Task<ResumoClienteDto> GetResumoCliente(string cpfCnpj)
        {
            var contratos = _contratosRepository.GetByCliente(cpfCnpj).ToList();
            var resumo = new ResumoClienteDto
            {
                ClienteCpfCnpj = cpfCnpj,
                QuantidadeContratosAtivos = contratos.Count,
                TotalParcelas = contratos.Sum(c => c.PrazoMeses),
                ParcelasPagas = contratos.Sum(c => c.Pagamentos.Count(p => p.Status == (int)StatusPagamento.EmDia || 
                                                                           p.Status == (int)StatusPagamento.Antecipado || 
                                                                           p.Status == (int)StatusPagamento.EmAtraso)),
                ParcelasEmAtraso = contratos.Sum(c => c.Pagamentos.Count(p => p.Status == (int)StatusPagamento.EmAtraso)),
                ParcelasAVencer = contratos.Sum(c => Math.Max(0, c.PrazoMeses - c.Pagamentos.Count)),
                PercentualPagasEmDia = 0,
                SaldoDevedorConsolidado = contratos.Sum(c => c.CalcularSaldoDevedorAtual())
            };

            var totalPagas = resumo.ParcelasPagas;
            var total = resumo.TotalParcelas;
            resumo.PercentualPagasEmDia = total == 0 ? 0 : (decimal)totalPagas / total * 100;

            return Task.FromResult(resumo);
        }
    }
}
