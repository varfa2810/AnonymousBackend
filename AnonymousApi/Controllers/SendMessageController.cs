using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/send-message")]
    [ApiController]
    [Authorize]
    public class SendMessageController : ControllerBase
    {
        private readonly ISendMessage _sendMessage;
        public SendMessageController(ISendMessage sendMessage)
        {
            _sendMessage = sendMessage;
        }

        [HttpPost("sendMessage")]
        public async Task<ActionResult<ApiResponse<int>>> SendMessage(
                                   [FromBody, SwaggerRequestBody("Message payload", Required = true)] SendMessageDto message)
        {
            var result = await _sendMessage.SendMessage(message);

            if (result > 0)
            {
                return Created("", new ApiResponse<int>
                {
                    Status = HttpStatusCode.Created,
                    Message = $"Message created successfully with Id {result}",
                    Data = result
                });
            }

            return BadRequest(new ApiResponse<int>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Message creation failed"
            });
        }
    }
}
