using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Data;
using PermissionsAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PermissionsAPI.Repositories.Permission;

public class PermissionRepository : Repository<PermissionEntity>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context) {}

    public async Task<IEnumerable<PermissionEntity>> GetPermissionsWithTypes()
    {
        return await _context.Permissions.Include(p => p.PermissionType).ToListAsync();
    }

    public async Task<PermissionEntity> GetPermissionByIdWithTypes(Guid id)
    {
        return await _context.Permissions.Include(x => x.PermissionType).FirstOrDefaultAsync(p => p.Id == id);
    }
}

