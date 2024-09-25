using PermissionsAPI.Models;
using System;

namespace PermissionsAPI.ElasticSearch;

public class PermissionIndexModel
{
    public Guid Id { get; set; }
    public required string TipoRequest { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? ApellidoEmpleado { get; set; }
    public Guid? TipoPermiso { get; set; }
    public DateTime? FechaPermiso { get; set; }
    public PermissionType? PermissionType { get; set; }
}

