using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Services
{
    public class SendMessageService : ISendMessage
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SendMessageService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<int> SendMessage(SendMessageDto message)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameter = new { Message = message.Message, UserId = message.UserId };
            var messageID = await connection.ExecuteScalarAsync<int>("SendMessage", parameter, commandType: CommandType.StoredProcedure);
            return messageID;
        }
    }
}
