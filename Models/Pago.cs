using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Display(Name = "Contrato")]
        public Contrato contrato { get; set; }

        
        public int IdContrato { get; set; }

        [Required, Display(Name = "Número de pago")]
        public int NroPago { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        
        public decimal Importe { get; set; }

    }
}
