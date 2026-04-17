using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class ApproveorRejectCompanyDto
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public bool Action { get; set; }
    }
}
