using AnonymousApplication.DTOs;
using Dapper;
using Microsoft.AspNetCore.Connections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public sealed class OutboxService : IOutbox
    {
        public async Task AddOutbox(CreateOutboxMessageDto message, IDbConnection connection, IDbTransaction transaction)
        {
            const string sql = @"
        INSERT INTO OutboxMessages
        (
            EventType,
            Payload,
            MessageStatus,
            CreatedAt,
            ProcessedAt,
            RetryCount
        )
        VALUES
        (
            @EventType,
            @Payload,
            @MessageStatus,
            @CreatedAt,
            @ProcessedAt,
            @RetryCount
        );";

            await connection.ExecuteAsync(sql, message, transaction);
        }
        public async Task<IEnumerable<OutboxMessageDto>> GetPendingMessages(IDbConnection connection)
        {

            const string sql = @"
        SELECT
            Id,
            EventType,
            Payload,
            MessageStatus,
            RetryCount,
            CreatedAt,
            ProcessedAt
        FROM OutboxMessages
        WHERE MessageStatus = 'Pending'
        ORDER BY CreatedAt;";

            return await connection.QueryAsync<OutboxMessageDto>(sql);
        }
        public async Task MarkAsProcessed(IDbConnection connection, int id)
        {

            const string sql = @"
        UPDATE OutboxMessages
        SET
            MessageStatus = 'Processed',
            ProcessedAt = GETUTCDATE()
        WHERE Id = @Id;";

            await connection.ExecuteAsync(sql, new { Id = id });
        }
        public async Task IncreaseRetryCount(IDbConnection connection, int id)
        {

            const string sql = @"
        UPDATE OutboxMessages
        SET RetryCount = RetryCount + 1
        WHERE Id = @Id;";

            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
