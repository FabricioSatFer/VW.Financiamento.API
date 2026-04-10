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

        public Task<bool> Create(ContratoCreateDto input)
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

            var created = _contratosRepository.Add(contrato);
            return Task.FromResult(created != null);
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
