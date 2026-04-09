using System;

namespace Financiamento.Domain.Entities
{
    public enum StatusPagamento { EmDia, EmAtraso, Antecipado }

    public class Pagamento
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public int ParcelaNumero { get; set; }
        public decimal ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataVencimento { get; set; }
        public int Status { get; set; }
    }
}
