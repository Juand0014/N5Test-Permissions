using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.CQRS.Queries;
using System;
using System.Security;
using System.Threading.Tasks;

namespace PermissionsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly CommandHandler commandHandler;
    private readonly QueryHandler queryHandler;
    private readonly IMediator mediator;

    public PermissionController(CommandHandler commandHandler, QueryHandler queryHandler, IMediator mediator)
    {
        this.commandHandler = commandHandler;
        this.queryHandler = queryHandler;
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RequestPermission([FromBody] AddPermissionCommand command)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var result = await commandHandler.Handle(command);
                return Ok(new { message = $"Element Added {result}" });

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.Message.ToString(),ex);
            }
        }
        return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ModifyPermission(Guid id, [FromBody] ModifyPermissionCommand command)
    {
        command.Id = id;
        var result = await commandHandler.Handle(command);
        return Ok(new
        {
            message = $"Modified with Id: {id}",
            model = result
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermissionById(Guid id)
    {
        var query = new GetPermissionByIdQuery((Guid)id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await queryHandler.Handle(new GetPermissionsQuery());
        return Ok(permissions);
    }
}
