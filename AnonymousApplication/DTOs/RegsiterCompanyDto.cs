using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{

    public class RegisterCompanyDto
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]

        public int EmployeeStrength { get; set; }

        [Required]

        public string Email { get; set; } = string.Empty;

        [Required]

        public string Phone { get; set; } = string.Empty;

        [Required]

        public int CountryId { get; set; }

        [Required]

        public int StateId { get; set; }

        [Required]

        public int CityId { get; set; }

        [Required]

        public string CompanyAddress { get; set; } = string.Empty;

        [Required]

        public string HREmail { get; set; } = string.Empty;
    }
}
