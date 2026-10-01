using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Paginacion
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Pagina { get; set; }
        public int Cantidad { get; set; }
        public int Total { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)Total / Cantidad);
    }
}