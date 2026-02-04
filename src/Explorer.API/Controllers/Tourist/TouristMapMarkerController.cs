using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/tourist/mapMarkers")]
    [ApiController]
    public class TouristMapMarkerController : ControllerBase
    {
        private readonly ITouristMapMarkerService _service;

        public TouristMapMarkerController(ITouristMapMarkerService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<PagedResult<TouristMapMarkerDto>> GetPagedByTourist(
            [FromQuery] long touristId,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = _service.GetPagedByTourist(page, pageSize, touristId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("all")]
        public ActionResult<List<TouristMapMarkerDto>> GetAllByTourist([FromQuery] long touristId)
        {
            try
            {
                var result = _service.GetAllByTourist(touristId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("active")]
        public ActionResult<TouristMapMarkerDto> GetActive([FromQuery] long touristId)
        {
            try
            {
                var result = _service.GetActiveByTourist(touristId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpPost("collect")]
        public ActionResult<TouristMapMarkerDto> CollectMapMarker(
            [FromQuery] long mapMarkerId)
        {
            var touristId = User.UserId();
            try
            {
                var dto = _service.Collect(touristId, mapMarkerId);
                return Ok(dto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpPost("activate")]
        public ActionResult<TouristMapMarkerDto> SetActive(
            [FromQuery] long mapMarkerId)
        {
            var touristId = User.UserId();
            try
            {
                var dto = _service.SetMapMarkerAsActive(touristId, mapMarkerId);
                return Ok(dto);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Don't really need delete
        //[Authorize(Policy = "touristPolicy")]
        //[HttpDelete("{id:long}")]
        //public IActionResult Delete(long id)
        //{
        //    try
        //    {
        //        _service.Delete(id);
        //        return NoContent();
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new { error = ex.Message });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}
    }
}
