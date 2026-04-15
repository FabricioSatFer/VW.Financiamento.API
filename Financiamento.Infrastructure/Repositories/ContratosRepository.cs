using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Data;
using Financiamento.Infrastructure.Interfaces;
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
            await _db.SaveChangesAsync();
            return contrato;
        }

        public async Task<Contrato> Get(Guid id) 
            => await _db.Contratos.Include(c => c.Pagamentos).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Contrato>> GetAll() 
            => await _db.Contratos.Include(c => c.Pagamentos).AsNoTracking().ToListAsync();

        public async Task<(IEnumerable<Contrato> Items, int TotalCount)> GetAllPaginado(int offset, int pageSize)
        {
            var totalCount = await _db.Contratos.CountAsync();
            var items = await _db.Contratos
                .Include(c => c.Pagamentos)
                .AsNoTracking()
                .OrderByDescending(c => c.DataVencimentoPrimeiraParcela)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task Remove(Guid id)
        {
            var c = await _db.Contratos.FindAsync(id);
            if (c == null) return;
            _db.Contratos.Remove(c);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contrato>> GetByCliente(string cpfCnpj) 
            => await _db.Contratos.Include(c => c.Pagamentos).Where(c => c.ClienteCpfCnpj == cpfCnpj).ToListAsync();

        public async Task Update(Contrato contrato)
        {
            _db.Contratos.Update(contrato);
            await _db.SaveChangesAsync();
        }
    }
}
