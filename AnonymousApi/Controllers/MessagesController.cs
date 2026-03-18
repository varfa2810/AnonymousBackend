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
    public class MessagesController : ControllerBase
    {
        private readonly IMessages _messages;
        public MessagesController(IMessages messages)
        {
            _messages = messages;
        }

        [HttpPost("react")]
        [SwaggerOperation(
     Summary = "React to Message",
     Description = "Adds or updates a reaction to a specific message."
 )]
        [ProducesResponseType(typeof(ApiResponse<ReactedDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ReactedDto>), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Reaction processed successfully", typeof(ApiResponse<ReactedDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or reaction failed")]
        public async Task<ActionResult<ApiResponse<bool>>> ReactToMessage(
     [FromBody, SwaggerRequestBody("Reaction payload", Required = true)] ReactToMessageDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            request.UserId = userId;
            var result = await _messages.ReactToMessage(request);

            return Ok(new ApiResponse<bool>
            {
                Status = HttpStatusCode.OK,
                Message = result ? "Reaction added." : "Reaction removed.",
                Data = result
            });

        }


        /// <summary>
        /// Get all messages.
        /// </summary>
        /// <returns>Returns list of all messages wrapped in ApiResponse.</returns>
        [HttpGet("getAllMessages")]
        [SwaggerOperation(
            Summary = "Get All Messages",
            Description = "Fetches all messages from the system."
        )]
        [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Messages fetched successfully", typeof(ApiResponse<List<MessageDto>>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Failed to fetch messages")]
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

            return BadRequest(new ApiResponse<List<MessageDto>>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Error in fetching messages.",
            });
        }
        /// <summary>
        /// Get all messages.
        /// </summary>
        /// <returns>Returns list of all messages wrapped in ApiResponse.</returns>
        [HttpGet("getMessageByUserId")]
        [SwaggerOperation(
            Summary = "Get All Messages by User id.",
            Description = "Fetches all messages from the system by User id."
        )]
        [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status200OK, "Messages fetched successfully", typeof(ApiResponse<List<MessageDto>>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Failed to fetch messages")]
        public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMessagesByUserId([Required] Guid userid)
        {
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

            return BadRequest(new ApiResponse<List<MessageDto>>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Error in fetching messages.",
            });
        }


        /// <summary>
        /// Deletes a message and its related reactions.
        /// </summary>
        /// <param name="messageId">Id of the message to delete.</param>
        [HttpDelete("deleteMessage")]
        [SwaggerOperation(
            Summary = "Delete Message",
            Description = "Deletes a message and all related reactions."
        )]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteMessage(
            [FromQuery, Required, SwaggerParameter("Message Id", Required = true)] int messageId)
        {
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

            return BadRequest(new ApiResponse<string>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Message not found or already deleted."
            });
        }
    }
}
