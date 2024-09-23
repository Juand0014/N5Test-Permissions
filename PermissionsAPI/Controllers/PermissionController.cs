using Microsoft.AspNetCore.Mvc;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.CQRS.Queries;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Services.Permission;

namespace PermissionsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionController : ControllerBase
{
    private readonly CommandHandler commandHandler;
    private readonly QueryHandler queryHandler;
    private readonly IKafkaProducer kafkaProducer;

    public PermissionController(IPermissionService permissionService, CommandHandler commandHandler, QueryHandler queryHandler, IKafkaProducer kafkaProducer)
    {
        this.commandHandler = commandHandler;
        this.queryHandler = queryHandler;
        this.kafkaProducer = kafkaProducer;
    }

    [HttpPost]
    public async Task<IActionResult> RequestPermission([FromBody] AddPermissionCommand command)
    {
        if (ModelState.IsValid)
        {
            await commandHandler.Handle(command);

            var kafkaMessage = new KafkaMessageDTO("Create permission");
            await kafkaProducer.SendMessageAsync("permissions-topic", kafkaMessage.Id.ToString(), kafkaMessage.Name);

            return Ok();
        }
        return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ModifyPermission(int id, [FromBody] ModifyPermissionCommand command)
    {
        command.Id = id;
        await commandHandler.Handle(command);

        var kafkaMessage = new KafkaMessageDTO("Modify Permission");
        await kafkaProducer.SendMessageAsync("permissions-topic", kafkaMessage.Id.ToString(), kafkaMessage.Name);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await queryHandler.Handle(new GetPermissionsQuery());

        var kafkaMessage = new KafkaMessageDTO("Get Permissions");
        await kafkaProducer.SendMessageAsync("permissions-topic", kafkaMessage.Id.ToString(), kafkaMessage.Name);

        return Ok(permissions);
    }
}
