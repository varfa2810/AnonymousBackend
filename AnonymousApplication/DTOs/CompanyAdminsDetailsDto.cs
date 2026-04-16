using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class CompanyAdminsDetailsDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Designation { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public int EmployeeStrength { get; set; }
        public string CompanyAddress { get; set; }
        public bool IsApproved { get; set; }
        public int BranchId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }

    }
}
