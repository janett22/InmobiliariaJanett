using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email  { get; set; }
        public string Dni { get; set; }
        public string Telefono  { get; set; }

        public String NombreGarante { get; set; }

        public String TelefonoGarante { get; set; }

        public String LugarDeTrabajo { get; set; }

    }
}
