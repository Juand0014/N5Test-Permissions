using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nest;
using PermissionsAPI.Common;
using PermissionsAPI.Constant;
using PermissionsAPI.CQRS.Queries;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using System;
using System.Threading.Tasks;

namespace PermissionsAPI.CQRS.Commands;

public class CommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer kafkaProducer;
    private readonly IElasticsearchService elasticsearchService;
    private readonly KafkaSettings kafkaSettings;
    private readonly IMediator mediator;

    public CommandHandler(IOptions<KafkaSettings> kafkaSettings ,IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer, IElasticsearchService elasticsearchService, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        this.kafkaProducer = kafkaProducer;
        this.elasticsearchService = elasticsearchService;
        this.kafkaSettings = kafkaSettings.Value;
        this.mediator = mediator;
    }

    public async Task<string> Handle(AddPermissionCommand command)
    {
        var permissionType = new PermissionType
        {
            Id = Guid.NewGuid(),
            Description = command.PermissionType.Description
        };

        var permission = new PermissionEntity
        {
            Id = Guid.NewGuid(),
            NombreEmpleado = command.NombreEmpleado,
            ApellidoEmpleado = command.ApellidoEmpleado,
            TipoPermiso = command.TipoPermiso ?? null,
            FechaPermiso = command.FechaPermiso,
            PermissionType = permissionType,
        };


        var permissionElastic = new PermissionIndexModel
        {
            Id = permission.Id,
            TipoRequest = "Requested",
            NombreEmpleado = permission.NombreEmpleado,
            ApellidoEmpleado = permission.ApellidoEmpleado,
            TipoPermiso = permission.TipoPermiso ?? null,
            FechaPermiso = DateTime.Now,
            PermissionType = permission.PermissionType
        };

        var noPermission = permission.TipoPermiso.HasValue & permission.PermissionType == null;
        if (noPermission)
            throw new NotFoundException("No found pemission type in the request");

        var hasPermissiosType = permission.TipoPermiso.HasValue;
        if (hasPermissiosType)
        {
            var query = new GetPermissionsTypeByIdQuery((Guid)permission.TipoPermiso);
            var result = await mediator.Send(query);

            if (result == null)
               throw new NotFoundException($"No found pemission type with id: {permission.TipoPermiso}");
        }

        await _unitOfWork.Permissions.Add(permission);
        await _unitOfWork.CompleteAsync();

        await elasticsearchService.IndexPermissionAsync(permissionElastic);

        var kafkaMessage = new KafkaMessageDTO(AppConstant.REQUESTPERMISSION);
        await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);

        return permission.Id.ToString();
    }

    public async Task<PermissionEntity> Handle(ModifyPermissionCommand command)
    {
        var permission = await _unitOfWork.Permissions.GetById((Guid)command.Id);

        if (permission == null)
            throw new NotFoundException($"Not found Permission with Id: {command.Id}");

        permission.Id = (Guid)command.Id;
        permission.NombreEmpleado = command.NombreEmpleado;
        permission.ApellidoEmpleado = command.ApellidoEmpleado;
        permission.TipoPermiso = command.TipoPermiso;
        permission.FechaPermiso = command.FechaPermiso;
        permission.PermissionType = command.PermissionType;

        var permissionElastic = new PermissionIndexModel
        {
            Id = (Guid)command.Id,
            TipoRequest = "Modified",
            NombreEmpleado = permission.NombreEmpleado,
            ApellidoEmpleado = permission.ApellidoEmpleado,
            TipoPermiso = permission.TipoPermiso,
            FechaPermiso = permission.FechaPermiso,
            PermissionType = command.PermissionType,
        };

        try
        {
            if (permission.TipoPermiso.HasValue)
            {
                var result = await mediator.Send(new GetPermissionsTypeByIdQuery((Guid)permission.TipoPermiso))
                    ?? throw new NotFoundException($"No found pemission type with id: {permission.TipoPermiso}");
            }
            
            if(permission.PermissionType != null)
            {
                _unitOfWork.PermissionTypes.Update(permission.PermissionType);
            }

            _unitOfWork.Permissions.Update(permission);
            await _unitOfWork.CompleteAsync();

            await elasticsearchService.IndexPermissionAsync(permissionElastic);

            var kafkaMessage = new KafkaMessageDTO(AppConstant.MODIFYPERMISSION);
            await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);

            return permission;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}

