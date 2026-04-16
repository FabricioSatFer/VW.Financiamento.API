using FluentValidation;
using Financiamento.Application.DTOs;
using Financiamento.Application.Helpers;
using Financiamento.Domain.Entities;
using System.Text.RegularExpressions;

namespace Financiamento.Application.Validators
{
    public class ContratoCreateValidator : AbstractValidator<ContratoCreateDto>
    {
        public ContratoCreateValidator()
        {
            RuleFor(x => x.ClienteCpfCnpj)
                .NotEmpty().WithMessage("CPF/CNPJ é obrigatório.")
                .Must(BeValidCpfOrCnpj).WithMessage("CPF/CNPJ inválido.");

            RuleFor(x => x.ValorTotal)
                .GreaterThan(5000).WithMessage("O valor mínimo para financiamento é R$ 5.000,00")
                .LessThanOrEqualTo(1000000).WithMessage("O valor máximo para financiamento é R$ 1.000.000,00");

            RuleFor(x => x.TaxaMensal)
                .GreaterThan(0).WithMessage("A taxa mensal deve ser maior que zero.")
                .LessThanOrEqualTo(10).WithMessage("A taxa mensal não pode ultrapassar 10%.");

            RuleFor(x => x.PrazoMeses)
                .GreaterThanOrEqualTo(1).WithMessage("O prazo mínimo é 1 mês.")
                .LessThanOrEqualTo(96).WithMessage("O prazo máximo é 96 meses.");

            RuleFor(x => x.DataVencimentoPrimeiraParcela)
                .GreaterThan(DateTime.Today).WithMessage("A data de vencimento da primeira parcela deve ser futura.");

            RuleFor(x => x.TipoVeiculo)
                .IsInEnum().WithMessage("Tipo de veículo inválido.");

            RuleFor(x => x.CondicaoVeiculo)
                .IsInEnum().WithMessage("Condição do veículo inválida.");

            // Validação Cross-Property: Prazo vs Tipo de Veículo
            RuleFor(x => x)
                .Must(x => ValidarPrazoPorVeiculo(x.TipoVeiculo, x.PrazoMeses))
                .WithMessage(x => $"Prazo não permitido para {x.TipoVeiculo}. Máximo: {ObterPrazoMaximo(x.TipoVeiculo)} meses.")
                .When(x => x.PrazoMeses > 0);
        }

        private bool BeValidCpfOrCnpj(string cpfCnpj)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                return false;

            var cleaned = Regex.Replace(cpfCnpj, @"[./-]", "").ToUpper();

            if (cleaned.Length == 11)
                return CPFHelper.Validar(cleaned);

            if (cleaned.Length == 14)
                return CNPJHelper.Validar(cleaned);

            return false;
        }

        private bool ValidarPrazoPorVeiculo(TipoVeiculo tipoVeiculo, int prazo)
        {
            int prazoMaximo = ObterPrazoMaximo(tipoVeiculo);
            return prazo >= 1 && prazo <= prazoMaximo;
        }

        private int ObterPrazoMaximo(TipoVeiculo tipoVeiculo)
        {
            return tipoVeiculo switch
            {
                TipoVeiculo.Automovel => 60,
                TipoVeiculo.Moto => 36,
                TipoVeiculo.Caminhao => 96,
                _ => 0
            };
        }
    }
}
