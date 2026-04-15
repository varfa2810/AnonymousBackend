using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Services
{
    public class HangfireService : IHangFire
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public HangfireService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task DeleteOldMessages()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                       DELETE TOP (500)
                       FROM Messages
                       WHERE CreatedDate < DATEADD(DAY, -30, GETUTCDATE())
                       ";

            var rows = await connection.ExecuteAsync(sql);

            Console.WriteLine($"Deleted {rows} old messages");
        }
    }
}
