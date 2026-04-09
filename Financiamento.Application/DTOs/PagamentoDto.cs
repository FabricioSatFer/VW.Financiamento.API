using System;
using Financiamento.Domain.Entities;

namespace Financiamento.Application.DTOs
{
    public class PagamentoDto
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public int ParcelaNumero { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public StatusPagamento Status { get; set; }
    }
}