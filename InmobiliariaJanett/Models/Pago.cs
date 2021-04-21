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
        public Contrato contrato { get; set; }
        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }
        [Required, Display(Name = "Número de pago")]
        public int NroPago { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public decimal Importe { get; set; }

    }
}
