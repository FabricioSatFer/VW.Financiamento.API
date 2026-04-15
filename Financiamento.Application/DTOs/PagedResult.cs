using System;
using System.Collections.Generic;

namespace Financiamento.Application.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanhoPagina);
        public bool TemAnterior => Pagina > 1;
        public bool TemProximo => Pagina < TotalPaginas;
    }
}
