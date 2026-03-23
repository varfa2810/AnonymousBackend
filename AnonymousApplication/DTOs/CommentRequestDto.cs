using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class CommentRequestDto
    {
        public int MessageId { get; set; }
        public string Comment { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
