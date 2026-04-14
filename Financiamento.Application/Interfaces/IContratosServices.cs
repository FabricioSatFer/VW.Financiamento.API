using Financiamento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.Interfaces
{
    public interface IContratosServices
    {
        Task<OperationResult<ContratoDto>> Create(ContratoCreateDto input);
        Task<ContratoDto?> Get(Guid id);
        Task<IEnumerable<ContratoDto>> GetAll();
        Task<bool> Delete(Guid id);
    }
}
