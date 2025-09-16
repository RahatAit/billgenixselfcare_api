using billgenixselfcare_api.Application.Features.Roles;
using billgenixselfcare_api.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace billgenixselfcare_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search, int? pageNumber, int? pageSize)
        {
            var result = await _mediator.Send(new GetAllUsersQuery
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            command.CreateBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(command);
            if (result.Success)
                return CreatedAtAction(nameof(GetAll), result);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            command.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteUserCommand { Id = id, DeleteBy = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
