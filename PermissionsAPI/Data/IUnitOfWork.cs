using PermissionsAPI.Repositories.Permission;
using PermissionsAPI.Repositories.PermissionTypes;
using System;
using System.Threading.Tasks;

namespace PermissionsAPI.Data;

public interface IUnitOfWork : IDisposable
{
    IPermissionRepository Permissions { get; }
    IPermissionTypeRepository PermissionTypes { get; }
    Task<int> CompleteAsync();
}
