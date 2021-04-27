using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int IdContrato { get; set; }
        [ForeignKey("IdContrato")]
        public Contrato Contrato { get; set; }

        [Display(Name = "N° Pago")]
        [Required]
        public int NroPago { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public decimal Importe { get; set; }

           }
}
