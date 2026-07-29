using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AnonymousApplication.Kafka;

namespace AnonymousApplication.Services
{
    public class SendMessageService : ISendMessage
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IOutbox _outbox;

        public SendMessageService(IDbConnectionFactory connectionFactory, IOutbox outbox)
        {
            _connectionFactory = connectionFactory;
            _outbox = outbox;
        }

        [EnableRateLimiting("messageLimiter")]
        public async Task<int> SendMessage(SendMessageDto message)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var parameter = new { Message = message.Message, UserId = message.UserId };
                var messageID = await connection.ExecuteScalarAsync<int>(
                    "SendMessage", parameter, transaction, commandType: CommandType.StoredProcedure);

                var outbox = new CreateOutboxMessageDto
                {
                    EventType = KafkaTopics.MessageCreated,
                    Payload = JsonSerializer.Serialize(message),
                    MessageStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    RetryCount = 0
                };

                await _outbox.AddOutbox(outbox, connection, transaction);
                transaction.Commit();

                return messageID;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
