using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
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

        private const decimal DESCONTO_ANTECIPADO_PERCENTUAL = 0.5m;
        private const decimal MULTA_ATRASO_PERCENTUAL = 2m;
        private const decimal JUROS_DIARIO_PERCENTUAL = 0.1m;
        private const int DIAS_JANELA_ANTECIPACAO = 5;

        public PagamentosServices(IPagamentosRepository pagamentosRepository, IContratosRepository contratosRepository)
        {
            _pagamentosRepository = pagamentosRepository;
            _contratosRepository = contratosRepository;
        }

        public async Task<PagamentoDto> RegistrarPagamento(Guid contratoId, PagamentoCreateDto input)
        {
            var contrato = await _contratosRepository.Get(contratoId);
            if (contrato == null) 
                throw new ArgumentException("Contrato não encontrado", nameof(contratoId));

            if (input.ParcelaNumero < 1 || input.ParcelaNumero > contrato.PrazoMeses)
                throw new ArgumentException($"Parcela inválida. O contrato possui {contrato.PrazoMeses} parcelas.", nameof(input.ParcelaNumero));

            var parcelaVencimento = contrato.DataVencimentoPrimeiraParcela.AddMonths(input.ParcelaNumero - 1);
            var valorOriginalParcela = CalcularValorParcela(contrato.ValorTotal, contrato.TaxaMensal, contrato.PrazoMeses);

            var (status, diasAntecipacao, diasAtraso) = CalcularStatus(input.DataPagamento.Date, parcelaVencimento.Date);
            var (valorDesconto, valorMulta, valorJuros) = CalcularAjustesFinanceiros(
                valorOriginalParcela, 
                status, 
                diasAntecipacao, 
                diasAtraso);

            var valorFinalParcela = valorOriginalParcela - valorDesconto + valorMulta + valorJuros;

            var pagamento = new Pagamento
            {
                ContratoId = contratoId,
                ParcelaNumero = input.ParcelaNumero,
                ValorOriginalParcela = valorOriginalParcela,
                ValorDesconto = valorDesconto,
                ValorJuros = valorJuros,
                ValorMulta = valorMulta,
                ValorPago = valorFinalParcela,
                DataPagamento = input.DataPagamento.ToUniversalTime(),
                DataVencimento = parcelaVencimento.ToUniversalTime(),
                Status = (int)status,
                DiasAntecipacao = diasAntecipacao,
                DiasAtraso = diasAtraso
            };

            var created = _pagamentosRepository.Add(pagamento);

            var dto = new PagamentoDto
            {
                Id = created.Id,
                ContratoId = created.ContratoId,
                ParcelaNumero = created.ParcelaNumero,
                ValorOriginalParcela = created.ValorOriginalParcela,
                ValorDesconto = created.ValorDesconto,
                ValorJuros = created.ValorJuros,
                ValorMulta = created.ValorMulta,
                ValorPago = created.ValorPago,
                DataPagamento = created.DataPagamento,
                DataVencimento = created.DataVencimento,
                Status = created.Status,
                DiasAntecipacao = created.DiasAntecipacao,
                DiasAtraso = created.DiasAtraso
            };

            return dto;
        }

        public async Task<IEnumerable<PagamentoDto>> GetByContrato(Guid contratoId)
        {
            var pagamentos = _pagamentosRepository.GetByContrato(contratoId)
                .Select(p => new PagamentoDto
                {
                    Id = p.Id,
                    ContratoId = p.ContratoId,
                    ParcelaNumero = p.ParcelaNumero,
                    ValorOriginalParcela = p.ValorOriginalParcela,
                    ValorDesconto = p.ValorDesconto,
                    ValorJuros = p.ValorJuros,
                    ValorMulta = p.ValorMulta,
                    ValorPago = p.ValorPago,
                    DataPagamento = p.DataPagamento,
                    DataVencimento = p.DataVencimento,
                    Status = p.Status,
                    DiasAntecipacao = p.DiasAntecipacao,
                    DiasAtraso = p.DiasAtraso
                });

            return await Task.FromResult(pagamentos);
        }

        private decimal CalcularValorParcela(decimal valorTotal, decimal taxaMensal, int prazoMeses)
        {
            if (taxaMensal == 0)
                return valorTotal / prazoMeses;

            var taxaDecimal = taxaMensal / 100m;
            var fatorPotencia = (decimal)Math.Pow((double)(1 + taxaDecimal), prazoMeses);
            var valorParcela = valorTotal * (taxaDecimal * fatorPotencia) / (fatorPotencia - 1);

            return Math.Round(valorParcela, 2);
        }

        private (StatusPagamento status, int diasAntecipacao, int diasAtraso) CalcularStatus(
            DateTime dataPagamento, 
            DateTime dataVencimento)
        {
            var diasDiferenca = (dataPagamento - dataVencimento).Days;

            if (diasDiferenca > 0)
            {
                return (StatusPagamento.EmAtraso, 0, diasDiferenca);
            }
            else if (diasDiferenca < -DIAS_JANELA_ANTECIPACAO)
            {
                return (StatusPagamento.Antecipado, Math.Abs(diasDiferenca), 0);
            }
            else
            {
                return (StatusPagamento.EmDia, 0, 0);
            }
        }

        private (decimal valorDesconto, decimal valorMulta, decimal valorJuros) CalcularAjustesFinanceiros(
            decimal valorOriginalParcela,
            StatusPagamento status,
            int diasAntecipacao,
            int diasAtraso)
        {
            decimal valorDesconto = 0;
            decimal valorMulta = 0;
            decimal valorJuros = 0;

            switch (status)
            {
                case StatusPagamento.Antecipado:
                    valorDesconto = Math.Round(valorOriginalParcela * (DESCONTO_ANTECIPADO_PERCENTUAL / 100m), 2);
                    break;

                case StatusPagamento.EmAtraso:
                    valorMulta = Math.Round(valorOriginalParcela * (MULTA_ATRASO_PERCENTUAL / 100m), 2);
                    valorJuros = Math.Round(valorOriginalParcela * (JUROS_DIARIO_PERCENTUAL / 100m) * diasAtraso, 2);
                    break;

                case StatusPagamento.EmDia:
                default:
                    break;
            }

            return (valorDesconto, valorMulta, valorJuros);
        }
    }
}
