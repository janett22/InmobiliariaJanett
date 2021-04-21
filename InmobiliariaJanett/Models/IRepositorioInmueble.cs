using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        Inquilino ObtenerPorEmail(string email);
    }
}
