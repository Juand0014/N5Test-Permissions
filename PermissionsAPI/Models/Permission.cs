using System;

namespace PermissionsAPI.Models;

public class PermissionEntity
{
    public Guid Id { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public DateTime? FechaPermiso { get; set; }
    public Guid? TipoPermiso { get; set; }
    public PermissionType? PermissionType { get; set; }
}
