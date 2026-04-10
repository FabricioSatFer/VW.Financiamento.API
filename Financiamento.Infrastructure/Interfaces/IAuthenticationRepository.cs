using Financiamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Infrastructure.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<Usuario?> ObterPorUsername(string username);
    }
}
