using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Data;
using Financiamento.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly FinanciamentoDbContext _db;
        public AuthenticationRepository(FinanciamentoDbContext db)
        {
            _db = db;
        }
        public async Task<Usuario> ObterPorUsername(string username)
            => await _db.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
    }
}
