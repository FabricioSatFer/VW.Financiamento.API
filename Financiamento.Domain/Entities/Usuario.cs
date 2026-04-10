using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Domain.Entities
{
    public enum Roles { ADMIN, CADASTRO }

    public class Usuario
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; } 
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public Roles RoleId { get; set; }
    }
}
