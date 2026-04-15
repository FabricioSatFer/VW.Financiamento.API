using System;

namespace Financiamento.Application.DTOs
{
    public class PaginationParameters
    {
        private const int MaxTamanhoPagina = 100;
        private int _tamanhoPagina = 10;

        public int Pagina { get; set; } = 1;

        public int TamanhoPagina
        {
            get => _tamanhoPagina;
            set => _tamanhoPagina = value > MaxTamanhoPagina ? MaxTamanhoPagina : value;
        }

        public int Offset => (Pagina - 1) * TamanhoPagina;
    }
}
