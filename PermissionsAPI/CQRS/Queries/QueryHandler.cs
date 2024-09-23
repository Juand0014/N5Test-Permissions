using Microsoft.Extensions.Options;
using PermissionsAPI.Constant;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Dto;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;

namespace PermissionsAPI.CQRS.Queries;

public class QueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IKafkaProducer kafkaProducer;
    private readonly IElasticsearchService elasticsearchService;
    private readonly KafkaSettings kafkaSettings;

    public QueryHandler(IOptions<KafkaSettings> kafkaSettings, IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer, IElasticsearchService elasticsearchService)
    {
        _unitOfWork = unitOfWork;
        this.kafkaProducer = kafkaProducer;
        this.elasticsearchService = elasticsearchService;
        this.kafkaSettings = kafkaSettings.Value;
    }

    public async Task<IEnumerable<PermissionEntity>> Handle(GetPermissionsQuery query)
    {
        var results = await _unitOfWork.Permissions.GetPermissionsWithTypes();
        var kafkaMessage = new KafkaMessageDTO(AppConstant.GETPERMISSION);
        await kafkaProducer.SendMessageAsync(kafkaSettings.Topic, kafkaMessage.Id.ToString(), kafkaMessage.Name);
        return results;
    }
}

