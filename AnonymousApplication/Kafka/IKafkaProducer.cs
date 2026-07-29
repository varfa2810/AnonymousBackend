using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync<T>(string topic, T message);
        Task PublishJsonAsync(string topic, string json);
    }
}
