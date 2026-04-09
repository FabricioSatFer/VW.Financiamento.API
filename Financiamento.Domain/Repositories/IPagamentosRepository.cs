using Financiamento.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Financiamento.Domain.Repositories
{
    public interface IPagamentosRepository
    {
        Pagamento Add(Pagamento pagamento);
        IEnumerable<Pagamento> GetByContrato(Guid contratoId);
    }
}
