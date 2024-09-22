namespace PermissionsAPI.CQRS.Commands;

public class ModifyPermissionCommand
{
    public int Id { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public int TipoPermiso { get; set; }
    public DateTime FechaPermiso { get; set; }
}

