using Financiamento.Domain.Entities;
using Financiamento.Domain.Repositories;
using Financiamento.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Financiamento.Infrastructure.Repositories
{
    public class ContratosRepository : IContratosRepository
    {
        private readonly FinanciamentoDbContext _db;

        public ContratosRepository(FinanciamentoDbContext db)
        {
            _db = db;
        }

        public async Task<Contrato> Add(Contrato contrato)
        {
            contrato.Id = contrato.Id == Guid.Empty ? Guid.NewGuid() : contrato.Id;
            _db.Contratos.Add(contrato);
            _db.SaveChanges();
            return contrato;
        }

        public Contrato Get(Guid id) => _db.Contratos.Include(c => c.Pagamentos).FirstOrDefault(c => c.Id == id);

        public IEnumerable<Contrato> GetAll() => _db.Contratos.Include(c => c.Pagamentos).AsNoTracking().ToList();

        public void Remove(Guid id)
        {
            var c = _db.Contratos.Find(id);
            if (c == null) return;
            _db.Contratos.Remove(c);
            _db.SaveChanges();
        }

        public IEnumerable<Contrato> GetByCliente(string cpfCnpj) => _db.Contratos.Include(c => c.Pagamentos).Where(c => c.ClienteCpfCnpj == cpfCnpj).ToList();

        public void Update(Contrato contrato)
        {
            _db.Contratos.Update(contrato);
            _db.SaveChanges();
        }
    }
}
