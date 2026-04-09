using System;
using System.Collections.Generic;

namespace Financiamento.Domain.Entities
{
    public enum TipoVeiculo { Automovel, Moto, Caminhao }
    public enum CondicaoVeiculo { Novo, Usado, Seminovo }

    public class Contrato
    {
        public Guid Id { get; set; }
        public string ClienteCpfCnpj { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal TaxaMensal { get; set; } // percentual
        public int PrazoMeses { get; set; }
        public DateTime DataVencimentoPrimeiraParcela { get; set; }
        public TipoVeiculo TipoVeiculo { get; set; }
        public CondicaoVeiculo CondicaoVeiculo { get; set; }

        public List<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

        public decimal CalcularSaldoDevedorAtual()
        {
            decimal totalPago = 0;
            foreach (var p in Pagamentos)
                totalPago += p.ValorPago;
            return Math.Max(0, ValorTotal - totalPago);
        }
    }
}
