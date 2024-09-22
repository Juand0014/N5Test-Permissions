using PermissionsAPI.Models;

namespace PermissionsAPI.Repositories.Permission;

public interface IPermissionRepository : IRepository<PermissionEntity>
{
    Task<IEnumerable<PermissionEntity>> GetPermissionsWithTypes();
}

