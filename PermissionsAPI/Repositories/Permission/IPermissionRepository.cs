using PermissionsAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PermissionsAPI.Repositories.Permission;

public interface IPermissionRepository : IRepository<PermissionEntity>
{
    Task<PermissionEntity> GetPermissionByIdWithTypes(Guid id);
    Task<IEnumerable<PermissionEntity>> GetPermissionsWithTypes();
}