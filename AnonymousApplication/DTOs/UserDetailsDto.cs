using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class UserDetailsDto
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public DateTime UserCreatedDate { get; set; }
        public string? RoleName { get; set; }
        public int? BranchId { get; set; }
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool? CompanyApproved { get; set; }
    }
}
