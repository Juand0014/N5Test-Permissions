namespace PermissionsAPI.Models;

public class PermissionEntity
{
    public int Id { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public DateTime FechaPermiso { get; set; }
    public int TipoPermiso { get; set; }
    public PermissionType? PermissionType { get; set; }
}
