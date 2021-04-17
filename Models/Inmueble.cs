using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        public string Direccion { get; set; }
        public int Ambientes { get; set; }
        public int Superficie{ get; set; }
        [Display(Name = "Dueño")]
        public int IdPropietario { get; set; }
        [ForeignKey("IdPropietario")]
        public Propietario Duenio { get; set; }
    }
}
