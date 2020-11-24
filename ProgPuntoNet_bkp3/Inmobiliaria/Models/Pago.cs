using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }
        [Required(ErrorMessage = "El Contrato es obligatorio")]
        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }
        [Required(ErrorMessage = "El Inquilino es obligatorio")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }
        [Required(ErrorMessage = "El Nro de Pago es obligatorio")]
        [Display(Name = "Nro de Pago")]
        public int NroPago { get; set; }
        [Required(ErrorMessage = "La Fecha de Pago es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; }
        [Required(ErrorMessage = "El Importe es obligatorio")]
        public decimal Importe { get; set; }
        [ForeignKey("IdContrato")]
        public Contrato Contrato { get; set; }
        [ForeignKey("IdInquilino")]
        public Inquilino Inquilino { get; set; }


    }
}
