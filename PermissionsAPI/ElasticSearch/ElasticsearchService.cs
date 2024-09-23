using Microsoft.Extensions.Options;
using Nest;
using PermissionsAPI.ElasticSearch.Interfaces;

namespace PermissionsAPI.ElasticSearch;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _elasticClient;

    public ElasticsearchService(IOptions<ElasticsearchSettings> settings)
    {
        var uri = new Uri(settings.Value.Uri);
        var connectionSettings = new ConnectionSettings(uri).DefaultIndex("permissions");
        _elasticClient = new ElasticClient(connectionSettings);
    }


    public async Task IndexPermissionAsync(PermissionIndexModel permission)
    {
        var response = await _elasticClient.IndexDocumentAsync(permission);
        if (!response.IsValid)
        {
            Console.WriteLine($"Error al indexar en Elasticsearch: {response.ServerError}");
        }
    }
}
