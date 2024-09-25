using PermissionsAPI.Data;
using PermissionsAPI.Models;

namespace PermissionsAPI.Repositories.PermissionTypes
{
    public class PermissionTypesRepository : Repository<PermissionType>, IPermissionTypeRepository
    {
        public PermissionTypesRepository(ApplicationDbContext context) : base(context) { }
    }
}
