using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{

    public class RegisterCompanyDto
    {
        public string CompanyName { get; set; } = string.Empty;

        public int EmployeeStrength { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public string CompanyAddress { get; set; } = string.Empty;

        public string HREmail { get; set; } = string.Empty;
    }
}
