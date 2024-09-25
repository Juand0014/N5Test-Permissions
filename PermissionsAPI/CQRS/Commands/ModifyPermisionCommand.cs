using PermissionsAPI.Models;
using System;

namespace PermissionsAPI.CQRS.Commands;

public class ModifyPermissionCommand
{
    public Guid? Id { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public Guid? TipoPermiso { get; set; }
    public DateTime FechaPermiso { get; set; }
    public PermissionType? PermissionType { get; set; }
}

