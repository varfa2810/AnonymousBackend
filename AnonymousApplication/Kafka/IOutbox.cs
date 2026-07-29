using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Kafka
{
    public interface IOutbox
    {
        Task AddOutbox(CreateOutboxMessageDto message, IDbConnection connection, IDbTransaction transaction);
        Task<IEnumerable<OutboxMessageDto>> GetPendingMessages(IDbConnection connection);

        Task MarkAsProcessed(IDbConnection connection, int id);

        Task IncreaseRetryCount(IDbConnection connection, int id);
    }
}
