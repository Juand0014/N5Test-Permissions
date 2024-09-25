using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using PermissionsAPI.Common;
using PermissionsAPI.Constant;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PermissionsAPI.CQRS.Queries;

public class QueryHandler : IRequestHandler<GetPermissionByIdQuery, PermissionEntity>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer kafkaProducer;
    private readonly IElasticsearchService elasticsearchService;
    private readonly KafkaSettings kafkaSettings;
    private readonly IMapper _mapper;

    public QueryHandler(IOptions<KafkaSettings> kafkaSettings, IUnitOfWork unitOfWork, IMapper mapper, IKafkaProducer kafkaProducer, IElasticsearchService elasticsearchService)
    {
        _unitOfWork = unitOfWork;
        this.kafkaProducer = kafkaProducer;
        this.elasticsearchService = elasticsearchService;
        this.kafkaSettings = kafkaSettings.Value;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PermissionEntity>> Handle(GetPermissionsQuery query)
    {
        var results = await _unitOfWork.Permissions.GetPermissionsWithTypes();
        var kafkaMessage = new KafkaMessageDTO(AppConstant.GETPERMISSION);
        await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);
        var permissionElastic = new PermissionIndexModel
        {
            Id = Guid.NewGuid(),
            TipoRequest = "Get all Info"
        };

        await elasticsearchService.IndexPermissionAsync(permissionElastic);
        return results;
    }

    public async Task<PermissionEntity> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permissionType = await _unitOfWork.Permissions.GetPermissionByIdWithTypes(request.Id);
        return _mapper.Map<PermissionEntity>(permissionType);
    }
}

