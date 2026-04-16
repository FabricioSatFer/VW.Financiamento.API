using Financiamento.Application.DTOs;
using Financiamento.Application.Helpers;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financiamento.Application.Services
{
    public class ClientesServices : IClientesServices
    {
        private readonly IContratosRepository _contratosRepository;

        public ClientesServices(IContratosRepository contratosRepository)
        {
            _contratosRepository = contratosRepository;
        }

        public async Task<OperationResult<ResumoClienteDto>> GetResumoCliente(string cpfCnpj)
        {
            var cpfCnpjLimpo = Regex.Replace(cpfCnpj, @"[./-]", "").ToUpper();
            var contratos = (await _contratosRepository.GetByCliente(cpfCnpjLimpo)).ToList();

            if (contratos == null || !contratos.Any())
            {
                return new OperationResult<ResumoClienteDto>
                {
                    Success = false,
                    Errors = new List<string> { $"Nenhum contrato encontrado para o cliente {cpfCnpj}" },
                    Code = "CLIENT_NOT_FOUND"
                };
            }

            var resumo = new ResumoClienteDto
            {
                ClienteCpfCnpj = cpfCnpjLimpo.Length == 11 
                                        ? CPFHelper.FormatarMascaraCPF(cpfCnpjLimpo) 
                                        : CNPJHelper.FormatarMascaraCNPJ(cpfCnpjLimpo),
                QuantidadeContratosAtivos = contratos.Count,
                TotalParcelas = contratos.Sum(c => c.PrazoMeses),
                ParcelasPagas = contratos.Sum(c => c.Pagamentos.Count(p => p.Status == (int)StatusPagamento.EmDia ||
                                                                           p.Status == (int)StatusPagamento.Antecipado ||
                                                                           p.Status == (int)StatusPagamento.EmAtraso)),
                ParcelasEmAtraso = contratos.Sum(c => c.Pagamentos.Count(p => p.Status == (int)StatusPagamento.EmAtraso)),
                ParcelasAVencer = contratos.Sum(c => Math.Max(0, c.PrazoMeses - c.Pagamentos.Count)),
                PercentualPagasEmDia = 0,
                SaldoDevedorConsolidado = contratos.Sum(c => c.CalcularSaldoDevedorAtual())
            };

            var totalPagas = resumo.ParcelasPagas;
            var total = resumo.TotalParcelas;
            resumo.PercentualPagasEmDia = total == 0 ? 0 : (decimal)totalPagas / total * 100;

            return new OperationResult<ResumoClienteDto>
            {
                Success = true,
                Value = resumo
            };
        }
    }
}
