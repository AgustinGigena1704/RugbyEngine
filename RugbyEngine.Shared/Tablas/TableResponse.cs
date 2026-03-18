using System;
using System.Collections.Generic;
using System.Text;

namespace RugbyEngine.Shared.Tablas
{
    public class TableResponse<T> where T : class
    {
        public required PaginacionDto Paginacion { get; set; }
        public required List<T> Registros { get; set; }
    }
}
