using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class CompanyDetailsResponseDto
    {

            public Guid CompanyId { get; set; }
            public string CompanyName { get; set; }
            public int EmployeeStrength { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string CompanyAddress { get; set; }
            public DateTime CompanyCreatedDate { get; set; }
            public DateTime CompanyUpdatedDate { get; set; }
            public bool IsApproved { get; set; }
            public string HREmail { get; set; }

            public int BranchId { get; set; }
            public DateTime BranchCreatedDate { get; set; }

            public string CountryName { get; set; }
            public string StateName { get; set; }
            public string CityName { get; set; }
        

    }
}
