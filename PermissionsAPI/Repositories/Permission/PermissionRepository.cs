using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Data;
using PermissionsAPI.Models;

namespace PermissionsAPI.Repositories.Permission;

public class PermissionRepository : Repository<PermissionEntity>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<PermissionEntity>> GetPermissionsWithTypes()
    {
        return await _context.Permissions.Include(p => p.PermissionType).ToListAsync();
    }
}

