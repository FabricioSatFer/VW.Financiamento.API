using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.DTOs
{
    public class LoginResponseDto
    {
        public string? Token { get; set; }
        public string? TokenType { get; set; }
        public int? ExpiresIn { get; set; }
    }
}
