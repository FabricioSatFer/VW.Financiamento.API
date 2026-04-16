using System;

namespace Financiamento.Application.DTOs
{
    public class PagamentoCreateDto
    {
        public int ParcelaNumero { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
