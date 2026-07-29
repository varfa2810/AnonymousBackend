using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Kafka
{

    public sealed class KafkaTopicInitializerHostedService
        : IHostedService
    {
        private readonly KafkaTopicInitializer _initializer;
        private readonly ILogger<KafkaTopicInitializerHostedService> _logger;

        public KafkaTopicInitializerHostedService(
            KafkaTopicInitializer initializer,
            ILogger<KafkaTopicInitializerHostedService> logger)
        {
            _initializer = initializer;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Initializing Kafka topics...");
            Console.WriteLine("Initializing Kafka topics...");

            try
            {
                await _initializer.InitializeAsync(cancellationToken);

                //_logger.LogInformation("Kafka topics initialized successfully.");
                Console.WriteLine("Kafka topics initialized successfully.");
            }
            catch (Exception ex)
            {
                //_logger.LogCritical(ex, "Unable to initialize Kafka topics.");
                Console.WriteLine("Unable to initialize Kafka topics.");

                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
