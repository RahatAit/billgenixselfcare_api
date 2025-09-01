using billgenixselfcare_api.Application.Services.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace billgenixselfcare_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "User.View")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        [Authorize(Policy = "User.Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            command.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(command);
            if (result.Success)
                return CreatedAtAction(nameof(GetAll), result);
            return BadRequest(result);
        }
    }
}
