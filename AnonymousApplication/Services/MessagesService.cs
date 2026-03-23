using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security.Claims;
using System.Text;

namespace AnonymousApplication.Services
{
    public class MessagesService : IMessages
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MessagesService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ReactToMessage(ReactToMessageDto reactToMessageDto)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameter = new
            {
                reactToMessageDto.MessageId,
                reactToMessageDto.UserId,
                reactToMessageDto.ReactionTypeId,
            };
            var result = await connection.ExecuteScalarAsync<bool>("ToggleReaction", parameter, commandType: CommandType.StoredProcedure);
            return result;
        }
        public async Task<List<MessageDto>> GetAllMessages(Guid userid)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                CurrentUserId = userid
            };

            var result = await connection.QueryAsync<MessageDto>(
                "GetAllMessagesWithReactions",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        public async Task<List<MessageDto>> GetMessagesByUserId(Guid userid)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                UserId = userid
            };

            var result = await connection.QueryAsync<MessageDto>(
                "GetUserMessagesWithReaction",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<bool> DeleteMessage(int messageId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var deleteReactionsQuery = @"
                DELETE FROM Reactions
                WHERE MessageId = @MessageId;";

                await connection.ExecuteAsync(
                    deleteReactionsQuery,
                    new { MessageId = messageId },
                    transaction
                );

                // 2️⃣ Delete message
                var deleteMessageQuery = @"
                DELETE FROM Messages
                WHERE Id = @MessageId;";

                var affectedRows = await connection.ExecuteAsync(
                    deleteMessageQuery,
                    new { MessageId = messageId },
                    transaction
                );

                transaction.Commit();

                return affectedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> CommentOnMessage(CommentRequestDto comment)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = @"INSERT INTO Comments (MessageId, Comment, UserId)
                     VALUES (@MessageId, @Comment, @UserId)";

            var parameters = new
            {
                MessageId = comment.MessageId,
                Comment = comment.Comment,
                UserId = comment.UserId,
            };

            var rowsaffected = await connection.ExecuteAsync(query, parameters);
            if (rowsaffected > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<CommentResponseDto>> GetCommentsByMessageId(int messageId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            string query = @"SELECT * FROM Comments WHERE MessageId = @messageId";

            var comments = await connection.QueryAsync<CommentResponseDto>(query, new { messageId });

            return comments.ToList();
        }
    }
}
