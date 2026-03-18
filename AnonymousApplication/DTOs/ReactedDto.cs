using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class ReactedDto
    {
        public int MessageId { get; set; }
        public string Message { get; set; }
        public int ReactionTypeId { get; set; }
        public string ReactionTypeName { get; set; }
        public int ReactionCount { get; set; }
    }
}
