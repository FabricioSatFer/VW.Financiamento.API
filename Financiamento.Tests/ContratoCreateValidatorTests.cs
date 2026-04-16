using System;
using System.Linq;
using Financiamento.Application.DTOs;
using Financiamento.Application.Validators;
using Financiamento.Domain.Entities;
using Xunit;

namespace Financiamento.Tests
{
    public class ContratoCreateValidatorTests
    {
        private readonly ContratoCreateValidator _validator;

        public ContratoCreateValidatorTests()
        {
            _validator = new ContratoCreateValidator();
        }

        [Fact]
        public void Valida_ContratoValido_DevePassar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Valida_CpfVazio_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ClienteCpfCnpj" 
                && e.ErrorMessage == "CPF/CNPJ é obrigatório.");
        }

        [Fact]
        public void Valida_CpfInvalido_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "123.456.789-00",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ClienteCpfCnpj" 
                && e.ErrorMessage == "CPF/CNPJ inválido.");
        }

        [Fact]
        public void Valida_CnpjValido_DevePassar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "11.222.333/0001-81",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Valida_ValorMenorQue5000_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 4999,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ValorTotal" 
                && e.ErrorMessage == "O valor mínimo para financiamento é R$ 5.000,00");
        }

        [Fact]
        public void Valida_ValorMaiorQue1000000_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 1000001,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ValorTotal" 
                && e.ErrorMessage == "O valor máximo para financiamento é R$ 1.000.000,00");
        }

        [Fact]
        public void Valida_TaxaMensalZero_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 0,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TaxaMensal" 
                && e.ErrorMessage == "A taxa mensal deve ser maior que zero.");
        }

        [Fact]
        public void Valida_TaxaMensalMaiorQue10_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 10.1m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "TaxaMensal" 
                && e.ErrorMessage == "A taxa mensal não pode ultrapassar 10%.");
        }

        [Fact]
        public void Valida_DataVencimentoPassada_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(-1),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "DataVencimentoPrimeiraParcela" 
                && e.ErrorMessage == "A data de vencimento da primeira parcela deve ser futura.");
        }

        [Fact]
        public void Valida_PrazoAutomovelAcimaDe60Meses_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 61,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Prazo não permitido para Automovel"));
        }

        [Fact]
        public void Valida_PrazoMotoAcimaDe36Meses_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 37,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Moto,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Prazo não permitido para Moto"));
        }

        [Fact]
        public void Valida_PrazoCaminhaoAte96Meses_DevePassar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 96,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Caminhao,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Valida_PrazoMenorQue1_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 0,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PrazoMeses" 
                && e.ErrorMessage == "O prazo mínimo é 1 mês.");
        }

        [Fact]
        public void Valida_PrazoMaiorQue96_DeveFalhar()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 50000,
                TaxaMensal = 1.5m,
                PrazoMeses = 97,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(30),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PrazoMeses" 
                && e.ErrorMessage == "O prazo máximo é 96 meses.");
        }

        [Fact]
        public void Valida_MultiplosCamposInvalidos_DeveRetornarTodosErros()
        {
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "",
                ValorTotal = 1000,
                TaxaMensal = 15,
                PrazoMeses = 0,
                DataVencimentoPrimeiraParcela = DateTime.Today.AddDays(-10),
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count >= 5);
        }
    }
}
