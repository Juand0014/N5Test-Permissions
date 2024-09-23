using Microsoft.Extensions.Options;
using Nest;
using PermissionsAPI.Constant;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;

namespace PermissionsAPI.CQRS.Commands;

public class CommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer kafkaProducer;
    private readonly IElasticsearchService elasticsearchService;
    private readonly KafkaSettings kafkaSettings;

    public CommandHandler(IOptions<KafkaSettings> kafkaSettings ,IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer, IElasticsearchService elasticsearchService)
    {
        _unitOfWork = unitOfWork;
        this.kafkaProducer = kafkaProducer;
        this.elasticsearchService = elasticsearchService;
        this.kafkaSettings = kafkaSettings.Value;
    }

    public async Task Handle(AddPermissionCommand command)
    {
        var permission = new PermissionEntity
        {
            NombreEmpleado = command.NombreEmpleado,
            ApellidoEmpleado = command.ApellidoEmpleado,
            TipoPermiso = command.TipoPermiso,
            FechaPermiso = command.FechaPermiso,
            PermissionType = command.PermissionType,
        };

        var permissionElastic = new PermissionIndexModel
        {
            Id = permission.Id,
            NombreEmpleado = permission.NombreEmpleado,
            ApellidoEmpleado = permission.ApellidoEmpleado,
            TipoPermiso = permission.TipoPermiso,
            FechaPermiso = DateTime.Now,
            PermissionType = permission.PermissionType,
        };

        await _unitOfWork.Permissions.Add(permission);
        await _unitOfWork.CompleteAsync();

        await elasticsearchService.IndexPermissionAsync(permissionElastic);

        var kafkaMessage = new KafkaMessageDTO(AppConstant.REQUESTPERMISSION);
        await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);
    }

    public async Task Handle(ModifyPermissionCommand command)
    {
        var permission = await _unitOfWork.Permissions.GetById(command.Id);
        if (permission != null)
        {
            permission.NombreEmpleado = command.NombreEmpleado;
            permission.ApellidoEmpleado = command.ApellidoEmpleado;
            permission.TipoPermiso = command.TipoPermiso;
            permission.FechaPermiso = command.FechaPermiso;
            permission.PermissionType = command.PermissionType;

            var permissionElastic = new PermissionIndexModel
            {
                Id = command.Id,
                NombreEmpleado = permission.NombreEmpleado,
                ApellidoEmpleado = permission.ApellidoEmpleado,
                TipoPermiso = permission.TipoPermiso,
                FechaPermiso = permission.FechaPermiso,
                PermissionType = permission.PermissionType,
            };

            _unitOfWork.Permissions.Update(permission);
            await _unitOfWork.CompleteAsync();

            await elasticsearchService.IndexPermissionAsync(permissionElastic);

            var kafkaMessage = new KafkaMessageDTO(AppConstant.MODIFYPERMISSION);
            await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);
        }
    }
}

