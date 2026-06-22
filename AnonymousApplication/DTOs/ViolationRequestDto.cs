using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public record ViolationRequestDto( 
        [Required]int MessageId, 
        [Required]int ViolatedOption, 
        string? Comment
        );
    
}
