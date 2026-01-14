using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [ApiController]
    [Route("api/author/tour-bundles")]
    public class TourBundleController : ControllerBase
    {
        private readonly ITourBundleRepository _repository;
        private readonly ITourRepository _tourRepository;
        private readonly ToursContext _dbContext;

        public TourBundleController(
            ITourBundleRepository repository,
            ITourRepository tourRepository,
            ToursContext dbContext)
        {
            _repository = repository;
            _tourRepository = tourRepository;
            _dbContext = dbContext;
        }

        // GET: /api/author/tour-bundles/my
        [HttpGet("my")]
        public ActionResult<List<TourBundle>> GetMyBundles()
        {
            try
            {
                var authorId = User.UserId();
                var bundles = _repository.GetByCreator(authorId);
                return Ok(bundles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: /api/author/tour-bundles/{id}
        [HttpGet("{id}")]
        public ActionResult<TourBundle> Get(long id)
        {
            try
            {
                var authorId = User.UserId();
                var bundle = _repository.Get(id);

                if (bundle.CreatorId != authorId)
                    return Forbid();

                return Ok(bundle);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: /api/author/tour-bundles
        [HttpPost]
        public ActionResult<TourBundle> Create([FromBody] CreateBundleRequest request)
        {
            try
            {
                var authorId = User.UserId();
                var tours = new List<Tour>();

                foreach (var tourId in request.TourIds)
                {
                    var tour = _tourRepository.Get(tourId);

                    if (tour.CreatorId != authorId)
                        return Forbid();

                    tours.Add(tour);
                }

                var bundle = new TourBundle(authorId, request.Name, tours, request.Price);
                _repository.Create(bundle);

                return Ok(bundle);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: /api/author/tour-bundles/{id}
        [HttpPut("{id}")]
        public ActionResult<TourBundle> Update(long id, [FromBody] UpdateBundleRequest request)
        {
            try
            {
                var authorId = User.UserId();
                var bundle = _repository.Get(id);

                if (bundle.CreatorId != authorId)
                    return Forbid();

                bundle.Update(request.Name, request.Price);
                _repository.Update(bundle);

                return Ok(bundle);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE: /api/author/tour-bundles/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var authorId = User.UserId();
                var bundle = _repository.Get(id);

                if (bundle.CreatorId != authorId)
                    return Forbid();

                if (bundle.Status == TourBundleStatus.Published)
                    return BadRequest(new { error = "Cannot delete a published bundle. Archive it first." });

                _repository.Delete(id);

                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: /api/author/tour-bundles/{id}/publish
        [HttpPut("{id}/publish")]
        public IActionResult Publish(long id)
        {
            try
            {
                var authorId = User.UserId();

                // Eksplicitno učitaj bundle sa svim Tours
                var bundle = _dbContext.Set<TourBundle>()
                    .Include(b => b.Tours)
                    .FirstOrDefault(b => b.Id == id)
                    ?? throw new NotFoundException($"Bundle {id} not found");

                if (bundle.CreatorId != authorId)
                    return Forbid();

                bundle.Publish();
                _dbContext.SaveChanges();

                return Ok(bundle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: /api/author/tour-bundles/{id}/archive
        [HttpPut("{id}/archive")]
        public IActionResult Archive(long id)
        {
            try
            {
                var authorId = User.UserId();

                // Eksplicitno učitaj bundle sa svim Tours
                var bundle = _dbContext.Set<TourBundle>()
                    .Include(b => b.Tours)
                    .FirstOrDefault(b => b.Id == id)
                    ?? throw new NotFoundException($"Bundle {id} not found");

                if (bundle.CreatorId != authorId)
                    return Forbid();

                bundle.Archive();
                _dbContext.SaveChanges();

                return Ok(bundle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT: /api/author/tour-bundles/{id}/activate
        [HttpPut("{id}/activate")]
        public IActionResult Activate(long id)
        {
            try
            {
                var authorId = User.UserId();

                // Eksplicitno učitaj bundle sa svim Tours
                var bundle = _dbContext.Set<TourBundle>()
                    .Include(b => b.Tours)
                    .FirstOrDefault(b => b.Id == id)
                    ?? throw new NotFoundException($"Bundle {id} not found");

                if (bundle.CreatorId != authorId)
                    return Forbid();

                bundle.Activate();
                _dbContext.SaveChanges();

                return Ok(bundle);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}