using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Net;

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

        [HttpPost("action")]
        public async Task<ActionResult<ApiResponse<int>>> ProcessCompanyRequest(ApproveorRejectCompanyDto request)
        {
            var result = await _superAdmin.ProcessCompanyRequest(request);

            if (result > 0)
            {
                return Ok(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Message = request.Action ? "Company approved successfully." : "Company rejected and deleted successfully.",
                    Data = request.Action
                });
            }

            return NotFound(new ApiResponse<string>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Company not found or no changes applied.",
                Data = "No data to return."
            });
        }

        [HttpGet("company-details")]
        public async Task<ActionResult<ApiResponse<List<CompanyDetailsResponseDto>>>> GetAllCompanyDetails(
           [FromQuery] bool? companyStatus, int pageNumber = 1, int pageSize = 10)
        {
            var details = await _superAdmin.GetAllCompanyDetails(pageNumber, pageSize, companyStatus);

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

        [HttpGet("user-details")]
        public async Task<ActionResult<ApiResponse<List<UserDetailsDto>>>> GetAllUsersDetails(
            [FromQuery] int? roleId, [FromQuery] bool? isCompanyApproved, int pageNumber = 1, int pageSize = 10)
        {
            if (roleId <= 0)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid role id.",
                });
            }

            var details = await _superAdmin.GetAllUsersDetails(roleId, isCompanyApproved, pageNumber, pageSize);

            if (details.Any())
            {
                return Ok(new ApiResponse<List<UserDetailsDto>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetch user details successfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<List<UserDetailsDto>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Not found any user.",
                Data = details
            });

        }


        [HttpGet("user-details/{userId}")]
        public async Task<ActionResult<ApiResponse<UserDetailsDto>>> GetUserDetailsById(Guid userid)
        {
            if (userid == Guid.Empty)
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid user id.",
                });
            }

            var details = await _superAdmin.GetUserDetailsById(userid);

            if (details != null)
            {
                return Ok(new ApiResponse<UserDetailsDto>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Fetch user details successfully.",
                    Data = details
                });
            }

            return NotFound(new ApiResponse<UserDetailsDto>
            {
                Status = HttpStatusCode.NotFound,
                Message = "Not found any user.",
                Data = details
            });

        }
    }
}
