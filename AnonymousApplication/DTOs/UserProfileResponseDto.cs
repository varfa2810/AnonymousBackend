using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class UserProfileResponseDto
    {
        public string Username { get; set; } = default!;
        public string Role { get; set; } = default!;
        public int BranchId { get; set; }
        public string CompanyName { get; set; } = default!;
        public int MessageContributionCount { get; set; }
        public int LikedCount { get; set; }
        public int DislikedCount { get; set; }
        public int HeartCount { get; set; }
        public int PartyCount { get; set; }

        
    }
}
