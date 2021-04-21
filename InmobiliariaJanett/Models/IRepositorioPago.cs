using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
   public interface IRepositorioPago : IRepositorio<Pago>
    {
        IList<Pago> BuscarPorContrato(int id);
    }
}
