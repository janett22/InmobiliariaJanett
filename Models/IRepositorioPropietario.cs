using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        Inquilino ObtenerPorEmail(string email);
    }
}
