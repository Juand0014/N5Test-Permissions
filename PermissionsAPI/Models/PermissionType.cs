using System;

namespace PermissionsAPI.Models;

public class PermissionType
{
    public Guid Id { get; set; }
    public required string Description { get; set; }
}
