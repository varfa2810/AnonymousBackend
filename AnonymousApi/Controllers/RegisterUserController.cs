using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/register")]
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        private readonly IRegisteruser _register;

        public RegisterUserController(IRegisteruser registeruser)
        {
            _register = registeruser;
        }


        [HttpPost("register-employee")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterEmployee([FromBody] RegisterUserDto request)
        {
            var result = await _register.RegisterEmployee(request);

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


        [HttpPost("register-admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterCompanyAdmin([FromBody] RegisterCompanyAdminDto request)
        {
            var result = await _register.RegisterCompanyadmin(request);

            if (!result)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Username already exists.",
                    Data = result
                });
            }

            return Ok(new ApiResponse<bool>
            {
                Status = HttpStatusCode.OK,
                Message = "Admin registered successfully.",
                Data = result
            });
        }


        [HttpPost("register-company")]
        [Authorize(Roles = "SuperAdmin, CompanyAdmin")]
        public async Task<ActionResult<ApiResponse<Guid>>> RegisterCompany(RegisterCompanyDto register)
        {
            var companyId = await _register.RegisterCompany(register);

            if (companyId != Guid.Empty)
            {
                return Created(
                    "",
                    new ApiResponse<Guid>
                    {
                        Status = System.Net.HttpStatusCode.OK,
                        Message = "Company registered successfully.",
                        Data = companyId
                    });
            }

            return BadRequest(new ApiResponse<string>
            {
                Status = System.Net.HttpStatusCode.BadRequest,
                Message = "Company registration failed.",
                Data = "No data to return."
            });
        }
    }
}
