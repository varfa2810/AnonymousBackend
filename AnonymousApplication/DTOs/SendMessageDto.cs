using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class SendMessageDto
    {
        [Required]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; } = Guid.Empty;
    }
}
