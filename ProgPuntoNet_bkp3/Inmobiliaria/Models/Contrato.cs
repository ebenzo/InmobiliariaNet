using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        public int IdContrato { get; set; }
        [Required(ErrorMessage = "La Descripcion es obligatoria")]
        [Display(Name = "Descripcion de contrato")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La Fecha de Inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "La Fecha de Fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }
        [Required(ErrorMessage = "El Monto es obligatorio")]
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "El Inquilino es obligatorio")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }
        public Inquilino Inquilino { get; set; }
        [Required(ErrorMessage = "El Inmueble es obligatorio")]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }
        public Inmueble Inmueble { get; set; }

    }
}
