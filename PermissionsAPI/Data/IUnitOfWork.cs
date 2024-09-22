using PermissionsAPI.Repositories.Permission;

namespace PermissionsAPI.Data;

public interface IUnitOfWork : IDisposable
{
    IPermissionRepository Permissions { get; }
    Task<int> CompleteAsync();
}
