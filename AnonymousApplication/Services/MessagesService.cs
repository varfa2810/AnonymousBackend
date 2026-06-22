using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using Microsoft.AspNetCore.RateLimiting;
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

        [EnableRateLimiting("reactionLimiter")]
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


            var deleteMessageQuery = @"
                DELETE FROM Messages
                WHERE Id = @MessageId;";

            var affectedRows = await connection.ExecuteAsync(
                deleteMessageQuery,
                new { MessageId = messageId }
            );


            return affectedRows > 0;

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

        public async Task<bool> ReportMessage(ViolationRequestDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@MessageId", request.MessageId);
            parameters.Add("@ViolationTypeId", request.ViolatedOption);
            parameters.Add("@Comment", request.Comment);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync(
                "[ReportMessage]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var result = parameters.Get<int>("@ReturnValue");
            return result == 0 ? true : false;
        }

        public async Task<ReportResponseDto> GetReportsAsync(int branchId, int pageNumber = 1, int pageSize = 10)
        {
            using var connection = _connectionFactory.CreateConnection();

            using var multi = await connection.QueryMultipleAsync(
                "[GetViolatedReports]",
                new { BranchId = branchId, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var reports = (await multi.ReadAsync<ReportDto>()).ToList();
            var totalRecords = await multi.ReadSingleAsync<int>();

            return new ReportResponseDto
            {
                Reports = reports,
                TotalRecords = totalRecords
            };
        }


    }
}
