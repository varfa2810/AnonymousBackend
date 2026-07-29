using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class KafkaOptions
    {
        public const string SectionName = "Kafka";

        public string BootstrapServers { get; init; } = string.Empty;

        public const string MessageCreated = "message.created.v1";

        public string GroupId { get; init; } = string.Empty;

        public string Acks { get; init; } = "All";

        public bool EnableIdempotence { get; init; }

        public int MessageTimeoutMs { get; init; }

        public int Retries { get; init; }
    }
}
