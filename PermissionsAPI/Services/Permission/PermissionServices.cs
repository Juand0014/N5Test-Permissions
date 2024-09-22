using PermissionsAPI.Models;
using PermissionsAPI.Repositories.Permission;

namespace PermissionsAPI.Services.Permission;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;

    public PermissionService(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<PermissionEntity>> GetAllPermissions()
    {
        return await _permissionRepository.GetPermissionsWithTypes();
    }

    public async Task<PermissionEntity> GetPermissionById(int id)
    {
        return await _permissionRepository.GetById(id);
    }

    public async Task AddPermission(PermissionEntity permission)
    {
        await _permissionRepository.Add(permission);
        await _permissionRepository.SaveChanges();
    }

    public async Task UpdatePermission(int id, PermissionEntity permission)
    {
        var existingPermission = await _permissionRepository.GetById(id);
        if (existingPermission != null)
        {
            existingPermission.NombreEmpleado = permission.NombreEmpleado;
            existingPermission.ApellidoEmpleado = permission.ApellidoEmpleado;
            existingPermission.TipoPermiso = permission.TipoPermiso;
            existingPermission.FechaPermiso = permission.FechaPermiso;

            _permissionRepository.Update(existingPermission);
            await _permissionRepository.SaveChanges();
        }
    }

    public async Task DeletePermission(int id)
    {
        var permission = await _permissionRepository.GetById(id);
        if (permission != null)
        {
            _permissionRepository.Delete(permission);
            await _permissionRepository.SaveChanges();
        }
    }
}

