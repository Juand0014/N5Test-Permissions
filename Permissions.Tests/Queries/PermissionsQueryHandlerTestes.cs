using Microsoft.Extensions.Options;
using Moq;
using PermissionsAPI.CQRS.Queries;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using PermissionsAPI.Repositories.Permission;
using PermissionsAPI.Kafka;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;

namespace Permissions.Tests.Queries;

public class PermissionsQueryHandlerTestes
{
    [Fact]
    public async Task Handle_Should_ReturnPermission_When_ValidIdIsProvided()
    {
        // Arrange: Crear los mocks de las dependencias
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var elasticsearchServicesMock = new Mock<IElasticsearchService>();
        var kafkaProducerMock = new Mock<IKafkaProducer>();
        var mapperMock = new Mock<IMapper>();

        // Mockear el repositorio de permisos dentro de UnitOfWork
        var permissionRepoMock = new Mock<IPermissionRepository>();
        unitOfWorkMock.Setup(u => u.Permissions).Returns(permissionRepoMock.Object);

        // Mockear la adición de permisos y la transacción
        permissionRepoMock.Setup(p => p.Add(It.IsAny<PermissionEntity>())).Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

        // Mockear la llamada a Elasticsearch
        elasticsearchServicesMock.Setup(x => x.IndexPermissionAsync(It.IsAny<PermissionIndexModel>()))
            .Returns(Task.CompletedTask);

        // Mockear la llamada a Kafka Producer
        kafkaProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Crear kafkaSettings
        var kafkaSettings = new KafkaSettings { Topic = "TestTopic", BootstrapServers = "localhost:9092" };
        var kafkaSettingsMock = Options.Create(kafkaSettings);

        // Simular un permiso existente en el repositorio
        var permissionsList = new List<PermissionEntity>
        {
            new PermissionEntity { Id = 1, NombreEmpleado = "Juan", ApellidoEmpleado = "Perez", TipoPermiso = 2, FechaPermiso = DateTime.Now.AddDays(-1) },
            new PermissionEntity { Id = 2, NombreEmpleado = "Maria", ApellidoEmpleado = "Gomez", TipoPermiso = 3, FechaPermiso = DateTime.Now }
        };

        unitOfWorkMock.Setup(u => u.Permissions).Returns(permissionRepoMock.Object);

        // Simular que la lista de permisos es devuelta cuando se llama a GetAll
        permissionRepoMock.Setup(p => p.GetPermissionsWithTypes())
            .ReturnsAsync(permissionsList);

        // Crear el query handler inyectando las dependencias mockeadas
        var handler = new QueryHandler(
            kafkaSettingsMock,
            unitOfWorkMock.Object,
            mapperMock.Object,
            kafkaProducerMock.Object,
            elasticsearchServicesMock.Object);

        // Crear un query para obtener todos los permisos
        var query = new GetPermissionsQuery();

        // Act: Ejecutar el handler
        var result = await handler.Handle(query);

        // Assert: Verificar que el resultado es la lista esperada
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());  // Verificar que se obtuvieron dos permisos

        // Verificar que se llamó al repositorio una vez
        permissionRepoMock.Verify(p => p.GetPermissionsWithTypes(), Times.Once);
    }
}
