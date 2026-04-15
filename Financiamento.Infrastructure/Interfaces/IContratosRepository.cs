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
        Task<Contrato> Add(Contrato contrato);
        Task<Contrato> Get(Guid id);
        Task<IEnumerable<Contrato>> GetAll();
        Task<(IEnumerable<Contrato> Items, int TotalCount)> GetAllPaginado(int offset, int pageSize);
        Task Remove(Guid id);
        Task<IEnumerable<Contrato>> GetByCliente(string cpfCnpj);
        Task Update(Contrato contrato);
    }
}
