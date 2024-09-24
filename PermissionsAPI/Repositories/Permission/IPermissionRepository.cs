using PermissionsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PermissionsAPI.Repositories.Permission;

public interface IPermissionRepository : IRepository<PermissionEntity>
{
    Task<IEnumerable<PermissionEntity>> GetPermissionsWithTypes();
    Task<PermissionType> GetPermissionTypeByIdAsync(int id);
}

