using System;
using Financiamento.Domain.Entities;

namespace Financiamento.Application.DTOs
{
    public class PagamentoDto
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public int ParcelaNumero { get; set; }
        public decimal ValorOriginalParcela { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorMulta { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public int Status { get; set; }
        public string StatusDescricao => ((StatusPagamento)Status).ToString();
        public int DiasAntecipacao { get; set; }
        public int DiasAtraso { get; set; }
    }
}