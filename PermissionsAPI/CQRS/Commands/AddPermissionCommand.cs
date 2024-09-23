using PermissionsAPI.Models;

namespace PermissionsAPI.CQRS.Commands;

public class AddPermissionCommand
{
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public int TipoPermiso { get; set; }
    public DateTime FechaPermiso { get; set; }
    public PermissionType? PermissionType { get; set; }
}
