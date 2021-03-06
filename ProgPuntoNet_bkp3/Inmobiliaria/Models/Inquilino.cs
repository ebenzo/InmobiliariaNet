﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        public int IdInquilino { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El dni es obligatorio")]
        public string Dni { get; set; }
        [Required(ErrorMessage = "El telefono es obligatorio")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "La direccion es obligatoria")]
        public string Direccion { get; set; }

        /*public List<Contrato> Contratos { get; set; }*/
    }
}
