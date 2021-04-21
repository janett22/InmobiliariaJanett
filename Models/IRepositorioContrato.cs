using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
   public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        Inquilino ObtenerPorEmail(string email);
    }
}
