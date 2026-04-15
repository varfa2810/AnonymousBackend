using AnonymousApplication.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Roles RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
