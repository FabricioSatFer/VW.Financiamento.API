using System;

namespace Financiamento.Application.DTOs
{
    public class PagamentoCreateDto
    {
        public int ParcelaNumero { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
