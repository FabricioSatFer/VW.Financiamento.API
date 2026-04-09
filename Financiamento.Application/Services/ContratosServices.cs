using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Domain.Entities;
using Financiamento.Domain.Repositories;

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

        public Task<ContratoDto?> Get(Guid id)
        {
            var c = _contratosRepository.Get(id);
            if (c == null) return Task.FromResult<ContratoDto?>(null);
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
            return Task.FromResult<ContratoDto?>(dto);
        }

        public Task<IEnumerable<ContratoDto>> GetAll()
        {
            var all = _contratosRepository.GetAll()
                .Select(c => new ContratoDto
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
            return Task.FromResult(all);
        }

        public Task<bool> Delete(Guid id)
        {
            var existing = _contratosRepository.Get(id);
            if (existing == null) return Task.FromResult(false);
            _contratosRepository.Remove(id);
            return Task.FromResult(true);
        }
    }
}
