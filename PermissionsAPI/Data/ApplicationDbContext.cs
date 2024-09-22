using Microsoft.EntityFrameworkCore;
using PermissionsAPI.Models;
using System.Security;

namespace PermissionsAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<PermissionType> PermissionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PermissionEntity>()
                    .HasOne(p => p.PermissionType)
                    .WithMany()
                    .HasForeignKey(p => p.TipoPermiso);

        base.OnModelCreating(modelBuilder);
    }
}
