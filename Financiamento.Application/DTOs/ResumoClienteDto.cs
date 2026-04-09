using System;

namespace Financiamento.Application.DTOs
{
    public class ResumoClienteDto
    {
        public string ClienteCpfCnpj { get; set; }
        public int QuantidadeContratosAtivos { get; set; }
        public int TotalParcelas { get; set; }
        public int ParcelasPagas { get; set; }
        public int ParcelasEmAtraso { get; set; }
        public int ParcelasAVencer { get; set; }
        public decimal PercentualPagasEmDia { get; set; }
        public decimal SaldoDevedorConsolidado { get; set; }
    }
}