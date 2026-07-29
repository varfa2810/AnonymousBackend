using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class CreateOutboxMessageDto
    {

        public string EventType { get; set; } = default!;

        public string Payload { get; set; } = default!;

        public string MessageStatus { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public int RetryCount { get; set; }
    }
}
