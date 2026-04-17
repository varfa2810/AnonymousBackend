using System.ComponentModel.DataAnnotations;
using System.Net;
using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousApi.Controllers
{
    [Route("api/super-admin")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ISuperAdmin _superAdmin;
        public SuperAdminController(ISuperAdmin superAdmin)
        {
            _superAdmin = superAdmin;
        }

        [HttpPost("approve/{companyId}")]
        public async Task<ActionResult<ApiResponse<int>>> ApproveCompany(Guid companyId, [FromQuery, Required] bool approve)
        {
            var result = await _superAdmin.ApproveOrRejectCompanyRequest(companyId, approve);

            if (result > 0)
            {
                if (approve)
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Status = HttpStatusCode.OK,
                        Message = "Company approved successfully.",
                        Data = true
                    });
                }

                else
                {
                    return Ok(new ApiResponse<bool>
                    {
                        Status = HttpStatusCode.OK,
                        Message = "Company rejected and deleted successfully.",
                        Data = false
                    });

                }
            }

            return NotFound(new ApiResponse<string>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Company not found or no changes applied.",
                Data = "No data to return."
            });
        }

        [HttpGet("company-details")]
        public async Task<ActionResult<ApiResponse<List<CompanyDetailsResponseDto>>>> GetAllCompanyDetails()
        {
            var details = await _superAdmin.GetAllCompanyDetails();

            if (details.Any())
            {
                return Ok(new ApiResponse<List<CompanyDetailsResponseDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetch all company details successfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<List<CompanyDetailsResponseDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Error fetching all company details.",
                Data = details
            });

        }


        [HttpGet("company-details/{companyId}")]
        public async Task<ActionResult<ApiResponse<CompanyDetailsResponseDto>>> GetAllCompanyDetailsWithId(Guid companyId)
        {
            var details = await _superAdmin.GetCompanyDetailsFromCompanyId(companyId);

            if (details != null)
            {
                return Ok(new ApiResponse<CompanyDetailsResponseDto>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetch company details successfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<CompanyDetailsResponseDto>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Error fetching company details.",
                Data = details
            });

        }

        [HttpGet("admins-details")]
        public async Task<ActionResult<ApiResponse<List<CompanyAdminsDetailsDto>>>> GetAllCompanyAdminsDetails()
        {
            var details = await _superAdmin.GetAllCompanyAdmins();

            if (details.Any())
            {
                return Ok(new ApiResponse<List<CompanyAdminsDetailsDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetched all company admins succesfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<List<CompanyAdminsDetailsDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No company admins found.",
                Data = details
            });
        }

        [HttpGet("admins-details/{companyId}")]
        public async Task<ActionResult<ApiResponse<List<CompanyAdminsDetailsDto>>>> GetCompanyAdminsFromCompanyId(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid company id."
                });
            }
            var details = await _superAdmin.GetCompanyAdminsFromCompanyId(companyId);

            if (details.Any())
            {
                return Ok(new ApiResponse<List<CompanyAdminsDetailsDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetched company admins succesfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<List<CompanyAdminsDetailsDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No admins founds for this company.",
                Data = details
            });
        }

    }
}
