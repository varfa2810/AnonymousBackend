using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/invite")]
    [ApiController]
    public class InviteController : ControllerBase
    {
        private readonly ICompanyAdminInvite _companyAdminInvite;
        private readonly ICompanyEmployeeInvite _companyEmployeeInvite;
        private readonly IInviteVerify _inviteVerify;

        public InviteController(ICompanyAdminInvite companyAdminInvite, ICompanyEmployeeInvite companyEmployeeInvite, IInviteVerify inviteVerify)
        {
            _companyAdminInvite = companyAdminInvite;
            _companyEmployeeInvite = companyEmployeeInvite;
            _inviteVerify = inviteVerify;
        }

        [HttpPost("{companyId}/cadmin-invite")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<string>>> CreateCompanyAdminInviteLink(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid companyId."
                });
            }

            string invitelink = await _companyAdminInvite.CreateCompanyAdminInviteLink(companyId);

            if (invitelink == string.Empty)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Error in creating invite link.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<string>
            {
                Status = HttpStatusCode.OK,
                Message = "Invitelink link created successfully.",
                Data = invitelink
            });
        }


        [HttpPost("{companyId}/cemployee-invite")]
        [Authorize(Roles = "SuperAdmin, CompanyAdmin")]
        public async Task<ActionResult<ApiResponse<string>>> CreateCompanyEmployeeInviteLink(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid companyId."
                });
            }

            string invitelink = await _companyEmployeeInvite.CreateCompanyEmployeeInviteLink(companyId);

            if (invitelink == string.Empty)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Error in creating invite link.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<string>
            {
                Status = HttpStatusCode.OK,
                Message = "Invitelink link created successfully.",
                Data = invitelink
            });
        }

        [HttpPost("verify-invite")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyInviteLink(string invitelink)
        {
            if (string.IsNullOrWhiteSpace(invitelink))
            {
                return BadRequest(new ApiResponse<dynamic>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid invite link."
                });
            }

            bool isValid = await _inviteVerify.VerifyInviteLink(invitelink);

            if (isValid)
            {
                return Ok(new ApiResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Link is valid.",
                    Data = isValid
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<dynamic>
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Link is expired.",
            });

        }
    }
}
