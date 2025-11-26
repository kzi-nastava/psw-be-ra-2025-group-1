using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Route("api/administration/users")]
    [ApiController]
    [Authorize(Policy = "administratorPolicy")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet]
        public ActionResult<List<AccountDto>> GetAll()
        {
            return Ok(_userManagementService.GetAll());
        }

        [HttpPut("{id}/block")]
        public ActionResult BlockUser(long id)
        {
            _userManagementService.BlockUser(id);
            return NoContent();
        }
    }
}
