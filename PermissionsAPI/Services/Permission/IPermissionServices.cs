using PermissionsAPI.Models;

namespace PermissionsAPI.Services.Permission;

public interface IPermissionService
{
    Task<IEnumerable<PermissionEntity>> GetAllPermissions();
    Task<PermissionEntity> GetPermissionById(int id);
    Task AddPermission(PermissionEntity permission);
    Task UpdatePermission(int id, PermissionEntity permission);
    Task DeletePermission(int id);
}

