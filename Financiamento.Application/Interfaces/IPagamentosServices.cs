using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financiamento.Application.DTOs;

namespace Financiamento.Application.Interfaces
{
    public interface IPagamentosServices
    {
        Task<PagamentoDto> RegistrarPagamento(Guid contratoId, PagamentoCreateDto input);
        Task<IEnumerable<PagamentoDto>> GetByContrato(Guid contratoId);
    }
}
