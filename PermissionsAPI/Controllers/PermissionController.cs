using Microsoft.AspNetCore.Mvc;
using Nest;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.CQRS.Queries;
using System.Threading.Tasks;

namespace PermissionsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly CommandHandler commandHandler;
    private readonly QueryHandler queryHandler;

    public PermissionController(CommandHandler commandHandler, QueryHandler queryHandler)
    {
        this.commandHandler = commandHandler;
        this.queryHandler = queryHandler;
    }

    [HttpPost]
    public async Task<IActionResult> RequestPermission([FromBody] AddPermissionCommand command)
    {
        if (ModelState.IsValid)
        {
            await commandHandler.Handle(command);
            return Ok();
        }
        return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ModifyPermission(int id, [FromBody] ModifyPermissionCommand command)
    {
        command.Id = id;
        await commandHandler.Handle(command);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await queryHandler.Handle(new GetPermissionsQuery());
        return Ok(permissions);
    }
}
