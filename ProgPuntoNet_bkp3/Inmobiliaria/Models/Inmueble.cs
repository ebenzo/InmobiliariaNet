using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        public int IdInmueble { get; set; }
        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; }
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El uso es obligatorio")]
        public string Uso { get; set; }
        [Required(ErrorMessage = "La cantidad de ambientes es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Min value must be 0")]
        public int Ambientes { get; set; }
        [Required(ErrorMessage = "La disponibilidad es obligatoria")]
        [Range(0, 1, ErrorMessage = "Can only be between 0 .. 1")] 
        public int Disponible { get; set; }
        [Required(ErrorMessage = "El precio es obligatorio")]
        public decimal Precio { get; set; }
        //[Required]
        //public int Habilitado { get; set; }
        [Required(ErrorMessage = "El propietario es obligatorio")]
        [Display(Name ="Propietario")]
        public int IdPropietario { get; set; }
        [ForeignKey("IdPropietario")]
        public Propietario Propietario { get; set; }
    }
}
