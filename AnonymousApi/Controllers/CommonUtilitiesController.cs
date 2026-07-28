using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/common-utility")]
    [ApiController]
    [AllowAnonymous]
    public class CommonUtilitiesController : ControllerBase
    {
        private readonly ICommonUtilities _utilities;
        public CommonUtilitiesController(ICommonUtilities utilities)
        {
            _utilities = utilities;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetCountries()
        {
            var countries = await _utilities.GetCountries();

            if (countries.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Countries fetched successfully.",
                    Data = countries
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No countries found.",
                Data = countries
            });
        }

        [HttpGet("states/{countryId}")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetStates(int countryId)
        {
            if (countryId <= 0)
            {
                return BadRequest(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid countryId provided.",
                    Data = null
                });
            }

            var states = await _utilities.GetStates(countryId);

            if (states.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "States fetched successfully.",
                    Data = states
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No state found.",
                Data = states
            });
        }

        [HttpGet("cities/{stateId}")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetCities(int stateId)
        {
            if (stateId <= 0)
            {
                return BadRequest(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = "Invalid stateId provided.",
                    Data = null
                });
            }

            var cities = await _utilities.GetCities(stateId);

            if (cities.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "Cities fetched successfully.",
                    Data = cities
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No city found.",
                Data = cities
            });
        }

        [HttpGet("roles")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetAllRoles()
        {
            var roles = await _utilities.GetAllRoles();

            if (roles.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "All roles fetched successfully.",
                    Data = roles
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No roles found.",
                Data = roles
            });
        }

        [HttpGet("company-admin-designation")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetAllCompanyAdminDesignation()
        {
            var cad = await _utilities.GetAllRoles();

            if (cad.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "All designation fetched.",
                    Data = cad
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No designation found.",
                Data = cad
            });
        }


        [HttpGet("violations-options")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetAllCommentViolationsOptions()
        {
            var options = await _utilities.GetAllCommentViolationsOptions();

            if (options.Any())
            {
                return Ok(new ApiResponse<List<dynamic>>
                {
                    Status = HttpStatusCode.OK,
                    Message = "All violations options fetched.",
                    Data = options
                });
            }

            return NotFound(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.NotFound,
                Message = "No violations options found.",
                Data = options
            });
        }
    }
}
