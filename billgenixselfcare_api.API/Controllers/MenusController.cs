using billgenixselfcare_api.API.CustomAttribute;
using billgenixselfcare_api.Application.Services.Menus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace billgenixselfcare_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [RequireDynamicPermission]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllMenusQuery());
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetMenuByIdQuery { Id = id });
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        [Authorize(Policy = "Menu.Create")]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommand command)
        {
            command.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(command);
            if (result.Success)
                return CreatedAtAction(nameof(GetAll), result);

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Menu.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            command.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Menu.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteMenuCommand { Id = id, UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
