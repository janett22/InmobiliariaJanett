using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public class Propietario
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni  { get; set; }
        public string Telefono  { get; set; }
        public string Email  { get; set; }
        public string clave  { get; set; }
        public string Avatar { get; set; }

        [NotMapped]
        public IFormFile AvatarFile { get; set; }


    }
}
