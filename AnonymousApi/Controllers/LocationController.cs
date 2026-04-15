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

            return BadRequest(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Failed to fetch countries.",
                Data = countries
            });
        }

        [HttpGet("states/{countryId}")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetStates(int countryId)
        {
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

            return BadRequest(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Failed to fetch states.",
                Data = states
            });
        }

        [HttpGet("cities/{stateId}")]
        public async Task<ActionResult<ApiResponse<List<dynamic>>>> GetCities(int stateId)
        {
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

            return BadRequest(new ApiResponse<List<dynamic>>
            {
                Status = HttpStatusCode.BadRequest,
                Message = "Failed to fetch cities.",
                Data = cities
            });
        }
    }
}
