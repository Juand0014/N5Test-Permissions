using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using PermissionsAPI.Controllers;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.CQRS.Queries;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using PermissionsAPI.Repositories.Permission;
using System.Threading.Tasks;
using Xunit;

namespace Permissions.Tests.Controllers;

public class PermissionsControllerTests
{
    [Fact]
    public async Task Post_Should_ReturnOk_When_ValidCommandIsPassed()
    {
        // Arrange: Crear mocks para las dependencias del CommandHandler
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var elasticsearchServicesMock = new Mock<IElasticsearchService>();
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var mapperMock = new Mock<IMapper>();
        var mediator = new Mock<IMediator>();

        // Mockear el repositorio para simular el comportamiento de agregar permisos
        var permissionsRepoMock = new Mock<IPermissionRepository>();
        unitOfWorkMock.Setup(u => u.Permissions).Returns(permissionsRepoMock.Object);
        permissionsRepoMock.Setup(p => p.Add(It.IsAny<PermissionEntity>())).Returns(Task.CompletedTask);

        // Mockear la transacción y completar el unit of work
        unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

        // Mockear el comportamiento del servicio de Elasticsearch
        elasticsearchServicesMock.Setup(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()))
            .Returns(Task.CompletedTask);

        // Mockear el comportamiento del productor de Kafka
        kafkaProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Crear kafkaSettings y mockear su objeto
        var kafkaSettings = new KafkaSettings { Topic = "TestTopic", BootstrapServers = "localhost:9092" };
        var kafkaSettingsMock = Options.Create(kafkaSettings);

        // Crear una instancia real de CommandHandler con las dependencias mockeadas
        var commandHandler = new CommandHandler(
            kafkaSettingsMock,
            unitOfWorkMock.Object,              
            kafkaProducerMock.Object,            
            elasticsearchServicesMock.Object,
            mediator.Object
        );

        var queryHandler = new QueryHandler(
            kafkaSettingsMock,
            unitOfWorkMock.Object,               
            mapperMock.Object,
            kafkaProducerMock.Object,           
            elasticsearchServicesMock.Object     
        );

        // Crear una instancia del controlador inyectando el CommandHandler real
        var controller = new PermissionController(commandHandler, queryHandler);

        // Crear un comando de prueba válido
        var command = new AddPermissionCommand
        {
            ApellidoEmpleado = "Perez",
            TipoPermiso = 15
        };

        // Act: Ejecutar el método Post del controlador
        var result = await controller.RequestPermission(command);

        // Assert: Verificar que el resultado es un 200 OK
        var okResult = Assert.IsType<OkResult>(result);
    }
}
   