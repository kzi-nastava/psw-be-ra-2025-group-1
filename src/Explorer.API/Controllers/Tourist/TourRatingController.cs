using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize]
    [Route("api/tourist/tour-ratings")]
    [ApiController]
    public class TourRatingController : ControllerBase
    {
        private readonly ITourRatingService _tourRatingService;
        private readonly ITourRatingReactionService _tourRatingReactionService;

        public TourRatingController(ITourRatingService tourRatingService, ITourRatingReactionService tourRatingReactionService)
        {
            _tourRatingService = tourRatingService;
            _tourRatingReactionService = tourRatingReactionService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourRatingDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_tourRatingService.GetPaged(page, pageSize));
        }

        [HttpGet("my-ratings")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<PagedResult<TourRatingDto>> GetMyRatings([FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                var userId = User.UserId();

                if (userId == 0) return Unauthorized(new { error = "Not logged in." });

                var result = _tourRatingService.GetPagedByUser(userId, page, pageSize);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "Not authorized." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving ratings.", details = ex.Message });
            }
        }

        [HttpGet("tour/{tourExecutionId:int}")]
        public ActionResult<PagedResult<TourRatingDto>> GetByTourExecution(int tourExecutionId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _tourRatingService.GetPagedByTourExecution(tourExecutionId, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<TourRatingDto> Create([FromBody] TourRatingDto rating)
        {
            try
            {
                var userId = User.UserId();
                if (userId == 0) return Unauthorized(new { error = "Not logged in." });

                rating.UserId = userId;

                var result = _tourRatingService.Create(rating);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the rating.", details = ex.Message });
            }
        }

        [HttpPut("{id:long}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<TourRatingDto> Update(long id, [FromBody] TourRatingDto rating)
        {
            try
            {
                var userId = User.UserId();
                if (userId == 0) return Unauthorized(new { error = "Not logged in." });

                rating.UserId = userId;

                var result = _tourRatingService.Update(id, rating);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the rating.", details = ex.Message });
            }
        }

        [HttpDelete("{id:long}")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult Delete(long id)
        {
            try
            {
                var userId = User.UserId();
                if (userId == 0) return Unauthorized(new { error = "Not logged in." });

                var user = User.UserId();

                _tourRatingService.Delete(id, user);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the rating.", details = ex.Message });
            }
        }

        [HttpPut("{id:long}/thumbs-up")]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<TourRatingDto> ThumbsUp(long id)
        {
            try
            {
                var userId = User.UserId();

                var updatedRating = _tourRatingReactionService.AddReaction(id, userId);
                return Ok(updatedRating);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the rating.", details = ex.Message });
            }
        }

        [HttpDelete("{id:long}/thumbs-up")]
        public ActionResult<TourRatingDto> RemoveThumbsUp(long id)
        {
            try
            {
                var userId = User.UserId();

                var updatedRating = _tourRatingReactionService.RemoveReaction(id, userId);
                return Ok(updatedRating);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the rating.", details = ex.Message });
            }
        }
    }
}