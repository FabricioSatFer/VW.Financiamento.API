using Financiamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Infrastructure.Interfaces
{
    public interface IContratosRepository
    {
        Contrato Add(Contrato contrato);
        Contrato Get(Guid id);
        IEnumerable<Contrato> GetAll();
        void Remove(Guid id);
        IEnumerable<Contrato> GetByCliente(string cpfCnpj);
        void Update(Contrato contrato);
    }
}
