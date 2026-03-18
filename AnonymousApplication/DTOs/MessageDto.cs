using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class MessageDto
    {
        public int MessageId { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public int Loves { get; set; }

        public int Party { get; set; }

        public bool IsUserLiked { get; set; }
        public bool IsUserDisliked { get; set; }
        public bool IsUserLoved { get; set; }
        public bool IsUserParty { get; set; }

    }

}
