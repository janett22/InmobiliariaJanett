using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class Contrato
    {

        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }
        [ForeignKey("InquilinoId")]
        public Inquilino Inquilino { get; set; }

        [Required]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }
        [ForeignKey("InmuebleId")]
        public Inmueble Inmueble { get; set; }
        [Required]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Required]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required]
        [Display(Name = "Monto Total")]
        public Decimal Precio { get; set; }
    
    }
}
