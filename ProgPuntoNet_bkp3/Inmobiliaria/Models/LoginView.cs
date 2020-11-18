using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class LoginView
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}
