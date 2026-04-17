using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class CommentRequestDto
    {
        [Required]
        public int MessageId { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
    }
}
