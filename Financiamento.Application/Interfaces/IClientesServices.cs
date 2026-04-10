using Financiamento.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.Interfaces
{
    public interface IClientesServices
    {
        Task<ResumoClienteDto> GetResumoCliente(string cpfCnpj);
    }
}
