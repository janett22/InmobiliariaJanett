using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaJanett.Models
{
    public enum enEstado 
    {
        Disponible = 1,
       NoDisponible = 2,
       EnRefaccion = 3,
    }

    public enum enTipo
    {
        Local = 1,
        Deposito = 2,
        Casa = 3,
        Departamento = 4,
    }

    public enum enUso
    {
        Comercial = 1,
        Residencial = 2,

    }

    public class Inmueble
    {
       
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        public string Direccion { get; set; }
        public int Uso { get; set; }

        public int Tipo { get; set; }
       
        public int Ambientes { get; set; }
       
        public decimal Precio { get; set; }
        public bool Estado { get; set; }

        public int Estado2 { get; set; }
        [Display(Name = "Dueño")]
        public int IdPropietario { get; set; }
        [ForeignKey("IdPropietario")]
        public Propietario Duenio { get; set; }


        [NotMapped]
        public string TipoNombre => Tipo > 0 ? ((enTipo)Tipo).ToString() : "";
        public static IDictionary<int, string> ObtenerTipos()
        {
            SortedDictionary<int, string> tipos = new SortedDictionary<int, string>();
            Type tipoEnum = typeof(enTipo);
            foreach (var valor in Enum.GetValues(tipoEnum))
            {
                tipos.Add((int)valor, Enum.GetName(tipoEnum, valor));
            }
            return tipos;
        }

        [NotMapped]
        public string UsoNombre => Uso > 0 ? ((enUso)Uso).ToString() : "";

        public static IDictionary<int, string> ObtenerUsos()
        {
            SortedDictionary<int, string> usos = new SortedDictionary<int, string>();
            Type usoEnum = typeof(enUso);
            foreach (var valor in Enum.GetValues(usoEnum))
            {
                usos.Add((int)valor, Enum.GetName(usoEnum, valor));
            }
            return usos;
        }


    }
}
