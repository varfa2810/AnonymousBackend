using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace AnonymousApi.Controllers
{
    [Route("api/messages")]
    [ApiController]
    [Authorize]
    public class MessagesController(IMessages messages) : ControllerBase
    {
        private readonly IMessages _messages = messages;

        [HttpPost("react")]
        public async Task<ActionResult<ApiResponse<bool>>> ReactToMessage([FromBody] ReactToMessageDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            request.UserId = userId;
            var result = await _messages.ReactToMessage(request);

            if (result)
            {
                return CreatedAtAction(nameof(GetMessagesByUserId), new ApiResponse<bool>
                {
                    Status = HttpStatusCode.Created,
                    Message = "reacted to comment successfully.",
                    Data = result
                });

            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Error in reacting.",
                Data = result
            });

        }


        [HttpGet("getAllMessages")]
        public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetAllMessages()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _messages.GetAllMessages(userId);

            if (result != null)
            {
                return Ok(new ApiResponse<List<MessageDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "All messages fetched successfully.",
                    Data = result
                });
            }

            return NotFound(new ApiResponse<List<MessageDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No message found.",
                Data = result
            });
        }


        [HttpGet("getMessageByUserId")]
        public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessagesByUserId([Required] Guid userid)
        {
            if (userid == Guid.Empty)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid userid provided."
                });
            }

            var result = await _messages.GetMessagesByUserId(userid);

            if (result != null)
            {
                return Ok(new ApiResponse<List<MessageDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "All messages fetched successfully.",
                    Data = result
                });
            }


            return NotFound(new ApiResponse<List<MessageDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No message found.",
                Data = result
            });
        }



        [HttpDelete("deleteMessage")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteMessage(
            [FromQuery, Required, SwaggerParameter("Message Id", Required = true)] int messageId)
        {
            if (messageId <= 0)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid messageid provided."
                });
            }

            var result = await _messages.DeleteMessage(messageId);

            if (result)
            {
                return Ok(new ApiResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Message deleted successfully.",
                    Data = "Deleted"
                });
            }

            return NotFound(new ApiResponse<string>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Message not found or already deleted."
            });
        }


        [HttpPost("commentOnMessage")]
        public async Task<ActionResult<ApiResponse<bool>>> CommentOnMessage(CommentRequestDto comment)
        {
            var result = await _messages.CommentOnMessage(comment);

            if (result)
            {
                return CreatedAtAction(nameof(GetCommentsByMessageId), new ApiResponse<bool>
                {
                    Status = HttpStatusCode.Created,
                    Message = "Comment inserted successfully.",
                    Data = result
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Error in inserting comment.",
                Data = result
            });
        }

        [HttpGet("getComments/{messageid}")]
        public async Task<ActionResult<ApiResponse<List<CommentResponseDto>>>> GetCommentsByMessageId(int messageid)
        {
            if (messageid <= 0)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid messageid provided."
                });
            }

            var comments = await _messages.GetCommentsByMessageId(messageid);

            if (comments == null || comments.Count == 0)
            {
                return NotFound(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.NotFound,
                    Message = "No comments found for this message.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<CommentResponseDto>>
            {
                Status = HttpStatusCode.OK,
                Message = "Fetched all comments.",
                Data = comments
            });
        }

        [HttpPost("reportMessage")]
        public async Task<ActionResult<ApiResponse<bool>>> ReportMessage(ViolationRequestDto request)
        {
            if (request.MessageId <= 0 || request.ViolatedOption <= 0)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Message id and Violation id cannot be 0.",
                    Data = false
                });
            }

            var result = await _messages.ReportMessage(request);

            if (result)
            {
                return Ok(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Message reported successfully.",
                    Data = result
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Error in reporting message.",
                Data = result
            });
        }

        [HttpGet("getReportedMessages/{branchId}")]
        public async Task<ActionResult<ApiResponse<ReportResponseDto>>> GetReports(int branchId, int pageNumber = 1, int pageSize = 10)
        {
            var reports = await _messages.GetReportsAsync(branchId, pageNumber, pageSize);

            if (reports.TotalRecords > 0)
            {
                return Ok(new ApiResponse<ReportResponseDto>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetched all reports.",
                    Data = reports
                });
            }

            return NotFound(new ApiResponse<int>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No reports found for this branch.",
                Data = reports.TotalRecords
            });

        }
    }
}
