using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class OutboxMessageDto
    {
        public int Id { get; set; }

        public string EventType { get; set; } = default!;

        public string Payload { get; set; } = default!;

        public string MessageStatus { get; set; } = default!;

        public int RetryCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
