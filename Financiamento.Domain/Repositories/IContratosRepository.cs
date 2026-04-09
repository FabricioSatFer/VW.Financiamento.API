using Financiamento.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Financiamento.Domain.Repositories
{
    public interface IContratosRepository
    {
        Task<Contrato> Add(Contrato contrato);
        Contrato Get(Guid id);
        IEnumerable<Contrato> GetAll();
        void Remove(Guid id);
        IEnumerable<Contrato> GetByCliente(string cpfCnpj);
        void Update(Contrato contrato);
    }
}
