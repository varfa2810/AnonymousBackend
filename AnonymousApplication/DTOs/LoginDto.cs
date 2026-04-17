using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is mandatory for login.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is mandatory for login.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }

}
