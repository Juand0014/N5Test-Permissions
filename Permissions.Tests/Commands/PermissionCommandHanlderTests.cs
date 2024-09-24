using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using PermissionsAPI.Constant;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using PermissionsAPI.Repositories.Permission;
using System;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Permissions.Tests.Commands;

public class PermissionCommandHanlderTests
{
    private Mock<IUnitOfWork> unitOfWorkMock;
    private Mock<IElasticsearchService> elasticsearchServicesMock;
    private Mock<IKafkaProducer> kafkaProducerMock;
    private Mock<IPermissionRepository> permissionsRepoMock;
    private IOptions<KafkaSettings> kafkaSettingsMock;
    private Mock<IMediator> mediator;
    private CommandHandler handler;

    public PermissionCommandHanlderTests()
    {
        SetUpMocks();
    }

    private void SetUpMocks()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        elasticsearchServicesMock = new Mock<IElasticsearchService>();
        kafkaProducerMock = new Mock<IKafkaProducer>();
        permissionsRepoMock = new Mock<IPermissionRepository>();

        unitOfWorkMock.Setup(u => u.Permissions).Returns(permissionsRepoMock.Object);

        var kafkaSettings = new KafkaSettings { Topic = "TestTopic", BootstrapServers = "localhost:9092" };
        kafkaSettingsMock = Options.Create(kafkaSettings);

        handler = new CommandHandler(
            kafkaSettingsMock,
            unitOfWorkMock.Object,
            kafkaProducerMock.Object,
            elasticsearchServicesMock.Object,
            mediator.Object
        );
    }


    [Fact]
    public async Task Handle_Should_IndexPermission_When_ValidCommandIsPassed()
    {
        // Arrange
        permissionsRepoMock.Setup(p => p.Add(It.IsAny<PermissionEntity>())).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

        elasticsearchServicesMock.Setup(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()))
            .Returns(Task.CompletedTask);

        var kafkaMessage = new KafkaMessageDTO(AppConstant.REQUESTPERMISSION + "_test");
        kafkaProducerMock.Setup(x => x.SendMessageAsync("permissions-topic", kafkaMessage.Id.ToString(), kafkaMessage.Name))
            .Returns(Task.CompletedTask);

        // Crear un comando de prueba
        var command = new AddPermissionCommand
        {
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "Perez",
            TipoPermiso = 2
        };

        // Act
        await handler.Handle(command);

        // Assert
        permissionsRepoMock.Verify(p => p.Add(It.IsAny<PermissionEntity>()), Times.Once);
        unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        elasticsearchServicesMock.Verify(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()), Times.Once);
        kafkaProducerMock.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_UpdatePermission_When_ValidCommandIsPassed()
    {
        // Arrange: Simular un permiso existente
        var existingPermission = new PermissionEntity
        {
            Id = 1,
            NombreEmpleado = "Juan",
            ApellidoEmpleado = "Perez",
            TipoPermiso = 2,
            FechaPermiso = DateTime.Now.AddDays(-1)
        };

        permissionsRepoMock.Setup(p => p.GetById(It.IsAny<int>()))
            .ReturnsAsync(existingPermission);

        permissionsRepoMock.Setup(p => p.Update(It.IsAny<PermissionEntity>()));
        unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

        elasticsearchServicesMock.Setup(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()))
            .Returns(Task.CompletedTask);

        kafkaProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Crear un comando de prueba para la actualización
        var command = new ModifyPermissionCommand
        {
            Id = 1,
            NombreEmpleado = "Juan Updated",
            ApellidoEmpleado = "Perez Updated",
            TipoPermiso = 2
        };

        // Act
        await handler.Handle(command);

        // Assert: Verificar que las llamadas correctas fueron hechas
        permissionsRepoMock.Verify(p => p.GetById(It.IsAny<int>()), Times.Once);
        permissionsRepoMock.Verify(p => p.Update(It.IsAny<PermissionEntity>()), Times.Once);
        unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once); 
        elasticsearchServicesMock.Verify(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()), Times.Once);  
        kafkaProducerMock.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once); 
    }
}
