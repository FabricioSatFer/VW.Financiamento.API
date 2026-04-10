using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
using Financiamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financiamento.Infrastructure.Repositories
{
    public class PagamentosRepository : IPagamentosRepository
    {
        private readonly FinanciamentoDbContext _db;

        public PagamentosRepository(FinanciamentoDbContext db)
        {
            _db = db;
        }

        public Pagamento Add(Pagamento pagamento)
        {
            pagamento.Id = pagamento.Id == Guid.Empty ? Guid.NewGuid() : pagamento.Id;
            _db.Pagamentos.Add(pagamento);
            _db.SaveChanges();
            return pagamento;
        }

        public IEnumerable<Pagamento> GetByContrato(Guid contratoId) => _db.Pagamentos.Where(p => p.ContratoId == contratoId).AsNoTracking().ToList();
    }
}
