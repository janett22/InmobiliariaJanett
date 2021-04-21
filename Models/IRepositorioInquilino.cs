using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        Inquilino ObtenerPorEmail(string email);
    }
}
