using PermissionsAPI.Repositories.Permission;
using System;
using System.Threading.Tasks;

namespace PermissionsAPI.Data;

public interface IUnitOfWork : IDisposable
{
    IPermissionRepository Permissions { get; }
    Task<int> CompleteAsync();
}
