using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class CommentResponseDto
    {
        public int MessageId { get; set; }
        public int CommentId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
