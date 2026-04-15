using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousApi.Controllers
{
    [Route("api/register-company")]
    [ApiController]
    [AllowAnonymous]
    public class RegsisterCompanyController : ControllerBase
    {
        private readonly IRegsiterCompany _regsiterCompany;
        public RegsisterCompanyController(IRegsiterCompany regsiterCompany)
        {
            _regsiterCompany = regsiterCompany;
        }

        [HttpPost("RegisterCompany")]
        public async Task<ActionResult<ApiResponse<Guid>>> RegisterCompany(RegisterCompanyDto register)
        {
            var companyId = await _regsiterCompany.RegisterCompany(register);

            if (companyId  != Guid.Empty)
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