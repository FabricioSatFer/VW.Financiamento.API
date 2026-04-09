using System;
using Financiamento.Domain.Entities;

namespace Financiamento.Application.DTOs
{
    public class ContratoCreateDto
    {
        public string ClienteCpfCnpj { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal TaxaMensal { get; set; }
        public int PrazoMeses { get; set; }
        public DateTime DataVencimentoPrimeiraParcela { get; set; }
        public TipoVeiculo TipoVeiculo { get; set; }
        public CondicaoVeiculo CondicaoVeiculo { get; set; }
    }
}
