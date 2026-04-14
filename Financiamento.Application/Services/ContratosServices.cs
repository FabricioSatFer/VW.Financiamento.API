using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;

namespace Financiamento.Application.Services
{
    public class ContratosServices : IContratosServices
    {
        private readonly IContratosRepository _contratosRepository;

        public ContratosServices(IContratosRepository contratosRepository)
        {
            _contratosRepository = contratosRepository;
        }

        public async Task<OperationResult<ContratoDto>> Create(ContratoCreateDto input)
        {
            try
            {
                var contrato = new Contrato
                {
                    Id = Guid.NewGuid(),
                    ClienteCpfCnpj = input.ClienteCpfCnpj,
                    ValorTotal = input.ValorTotal,
                    TaxaMensal = input.TaxaMensal,
                    PrazoMeses = input.PrazoMeses,
                    DataVencimentoPrimeiraParcela = input.DataVencimentoPrimeiraParcela,
                    TipoVeiculo = input.TipoVeiculo,
                    CondicaoVeiculo = input.CondicaoVeiculo
                };

                var created = await _contratosRepository.Add(contrato);
                if (created != null)
                {
                    var dto = new ContratoDto
                    {
                        Id = created.Id,
                        ClienteCpfCnpj = created.ClienteCpfCnpj,
                        ValorTotal = created.ValorTotal,
                        TaxaMensal = created.TaxaMensal,
                        PrazoMeses = created.PrazoMeses,
                        DataVencimentoPrimeiraParcela = created.DataVencimentoPrimeiraParcela,
                        TipoVeiculo = created.TipoVeiculo,
                        CondicaoVeiculo = created.CondicaoVeiculo
                    };

                    return new OperationResult<ContratoDto>
                    {
                        Success = true,
                        Value = dto
                    };
                }

                return new OperationResult<ContratoDto>
                {
                    Success = false,
                    Errors = new[] { "Erro ao incluir contrato." }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<ContratoDto>
                {
                    Success = false,
                    Errors = new[] { $"Erro ao incluir contrato: {ex.Message}" }
                };
            }
        }

        public async Task<ContratoDto?> Get(Guid id)
        {
            var c = await _contratosRepository.Get(id);
            if (c == null) return null;

            var dto = new ContratoDto
            {
                Id = c.Id,
                ClienteCpfCnpj = c.ClienteCpfCnpj,
                ValorTotal = c.ValorTotal,
                TaxaMensal = c.TaxaMensal,
                PrazoMeses = c.PrazoMeses,
                DataVencimentoPrimeiraParcela = c.DataVencimentoPrimeiraParcela,
                TipoVeiculo = c.TipoVeiculo,
                CondicaoVeiculo = c.CondicaoVeiculo
            };
            return dto;
        }

        public async Task<IEnumerable<ContratoDto>> GetAll()
            => (await _contratosRepository.GetAll()).Select(c => new ContratoDto
            {
                Id = c.Id,
                ClienteCpfCnpj = c.ClienteCpfCnpj,
                ValorTotal = c.ValorTotal,
                TaxaMensal = c.TaxaMensal,
                PrazoMeses = c.PrazoMeses,
                DataVencimentoPrimeiraParcela = c.DataVencimentoPrimeiraParcela,
                TipoVeiculo = c.TipoVeiculo,
                CondicaoVeiculo = c.CondicaoVeiculo
            });

        public async Task<bool> Delete(Guid id)
        {
            var existing = await _contratosRepository.Get(id);
            if (existing == null) return false;
            await _contratosRepository.Remove(id);
            return true;
        }
    }
}
