using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/register")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterUserController : ControllerBase
    {
        private readonly IRegisteruser _register;

        public RegisterUserController(IRegisteruser registeruser)
        {
            _register = registeruser;
        }


        [HttpPost("register-employee")]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterEmployee([FromBody] RegisterUserDto request)
        {
            var result = await _register.RegisterEmployee(request);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error in registering  employee.",
                    Data = result
                });
            }

            return Created("", new ApiResponse<bool>
            {
                Status = HttpStatusCode.Created,
                Message = "Employee registered successfully.",
                Data = result
            });
        }


        [HttpPost("register-admin")]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterCompanyAdmin([FromBody] RegisterCompanyAdminDto request)
        {
            var result = await _register.RegisterCompanyadmin(request);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<bool>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Error in registering admin.",
                    Data = result
                });
            }

            return Created("", new ApiResponse<bool>
            {
                Status = HttpStatusCode.Created,
                Message = "Company admin registered successfully.",
                Data = result
            });
        }


        [HttpPost("register-company")]
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

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<dynamic>
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Company registration failed.",
            });
        }
    }
}
