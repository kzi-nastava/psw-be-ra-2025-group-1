using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Route("api/monument")]
    [ApiController]
    public class MonumentController : ControllerBase
    {
        private readonly IMonumentService _monumentService;
        public MonumentController(IMonumentService monumentService)
        {
            _monumentService = monumentService;
        }

        [HttpGet("paged-all")]
        [Authorize(Policy = "userPolicy")]
        public ActionResult<PagedResult<MonumentDto>> GetAllPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_monumentService.GetPaged(page, pageSize));
        }

        [HttpGet("all")]
        [Authorize(Policy = "userPolicy")]
        public ActionResult<List<MonumentDto>> GetAll()
        {
            return Ok(_monumentService.GetAll());
        }

        [HttpGet("{id:long}")]
        [Authorize (Policy = "userPolicy")]
        public ActionResult<MonumentDto> GetById(long id)
        {
            return Ok(_monumentService.GetById(id));
        }

        [HttpPost]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult<MonumentDto> Create([FromBody] MonumentDto monument)
        {
            return Ok(_monumentService.Create(monument));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult<MonumentDto> Update(long id, [FromBody] MonumentDto monument)
        {
            monument.Id = id;
            return Ok(_monumentService.Update(monument));
        }

        [HttpDelete("{id:long}")]
        [Authorize(Policy = "administratorPolicy")]
        public ActionResult Delete(long id)
        {
            _monumentService.Delete(id);
            return Ok();
        }

    }
}
