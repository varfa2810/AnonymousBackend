using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class ReactToMessageDto
    {
        [Required]
        public int MessageId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int ReactionTypeId { get; set; }
    }
}
