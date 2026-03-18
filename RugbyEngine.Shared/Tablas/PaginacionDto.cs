using System;
using System.Collections.Generic;
using System.Text;

namespace RugbyEngine.Shared.Tablas
{
    public class PaginacionDto
    {
        public int Pagina { get; set; } = 1;
        public int Total { get; set; }
        public int RegistrosPorPagina { get; set; } = 10;
        public int TotalPaginas => (int)Math.Ceiling((decimal)Total / RegistrosPorPagina);
    }
}
