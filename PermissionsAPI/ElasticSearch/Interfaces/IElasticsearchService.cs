namespace PermissionsAPI.ElasticSearch.Interfaces;

public interface IElasticsearchService
{
    Task IndexPermissionAsync(PermissionIndexModel permission);
}
