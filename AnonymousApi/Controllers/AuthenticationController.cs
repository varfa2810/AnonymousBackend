using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserAuthentication _userAuthentication;

        public AuthenticationController(IUserAuthentication userAuthentication)
        {
            _userAuthentication = userAuthentication;
        }



        /// <summary>
        /// Login and get JWT token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("loginLimiter")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto request)
        {
            var result = await _userAuthentication.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Message = "Invalid username or password"
                });
            }

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = result.Expiration
            });

            return Ok(new ApiResponse<LoginResponseDto>
            {
                Status = HttpStatusCode.OK,
                Message = "Login successful",
                Data = result
            });
        }


        [HttpGet("WhoAmI")]
        [Authorize]
        public async Task<IActionResult> WhoAmI()
        {

            var user = new
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value,
                Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
            };

            return Ok(user);
        }


        [HttpGet("CheckUniqueUsername")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUniqueUsername([Required] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid username provided."
                });
            }

            var result = await _userAuthentication.CheckUniqueUsername(username);

            if (result)
            {
                return Ok(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Username exist.",
                    Data = result
                });
            }

            return NotFound(new ApiResponse<bool>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Username does not exist.",
                Data = result
            });
        }

        [Authorize]
        [HttpDelete("deleteUser/{userId}")]
        [EnableRateLimiting("deleteLimiter")]
        public async Task<ActionResult<ApiResponse<int>>> DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid userId provided."
                });
            }

            var result = await _userAuthentication.DeleteUser(userId);

            if (result > 0)
            {
                Response.Cookies.Delete("access_token", new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });

                return Ok(new ApiResponse<int>
                {
                    Status = HttpStatusCode.OK,
                    Message = "User deleted successfully",
                    Data = result
                });
            }

            return NotFound(new ApiResponse<int>
            {
                Status = HttpStatusCode.NotFound,
                Message = "User not found",
                Data = result
            });

        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
            return Ok(new ApiResponse<bool>
            {
                Status = HttpStatusCode.OK,
                Message = "Logged out successfully.",
                Data = true
            });
        }
    }
}
