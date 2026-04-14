using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.DTOs
{
    public class LoginRequestDto
    {
        public string? Username { get; set; }
        public string? Senha { get; set; }
    }
}
