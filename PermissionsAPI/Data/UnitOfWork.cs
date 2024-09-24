using PermissionsAPI.Repositories.Permission;
using System.Threading.Tasks;

namespace PermissionsAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IPermissionRepository Permissions { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Permissions = new PermissionRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
