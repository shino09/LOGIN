using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Login.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Dirección de Correo electrónico incorrecta.")]
        [StringLength(100, ErrorMessage = "Longitud máxima 100")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        //[StringLength(15, ErrorMessage = "Longitud entre 6 y 15 caracteres.",
        //              MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}