using AnonymousApplication.Interfaces;
using AnonymousApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AnonymousApi.Controllers
{
    [Route("api/location")]
    [ApiController]
    [AllowAnonymous]
    public class LocationController : ControllerBase
    {
        private readonly ILocation _location;
        public LocationController(ILocation location)
        {
            _location = location;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetCountries()
        {
            var countries = await _location.GetCountries();

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

            var states = await _location.GetStates(countryId);

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

            var cities = await _location.GetCities(stateId);

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
    }
}
