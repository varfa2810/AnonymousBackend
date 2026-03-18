using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
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
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Register new user",
            Description = "Creates a new user account with hashed password."
        )]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Register(
            [FromBody] RegisterUserDto request)
        {
            var result = await _userAuthentication.RegisterUser(request);

            if (!result)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Username already exists.",
                });
            }

            return Ok(new ApiResponse<string>
            {
                Status = HttpStatusCode.OK,
                Message = "User registered successfully.",
                Data = "Success"
            });
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
            };

            return Ok(user);
        }


        [HttpGet("CheckUniqueUsername")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUniqueUsername([Required] string username)
        {

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
