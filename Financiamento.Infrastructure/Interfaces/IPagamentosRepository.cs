using Financiamento.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Infrastructure.Interface
{
    public interface IPagamentosRepository
    {
        Pagamento Add(Pagamento pagamento);
        IEnumerable<Pagamento> GetByContrato(Guid contratoId);
    }
}
