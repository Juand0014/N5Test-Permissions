using PermissionsAPI.Data;
using PermissionsAPI.Models;
using System.Security;

namespace PermissionsAPI.CQRS.Commands;

public class CommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddPermissionCommand command)
    {
        var permission = new PermissionEntity
        {
            NombreEmpleado = command.NombreEmpleado,
            ApellidoEmpleado = command.ApellidoEmpleado,
            TipoPermiso = command.TipoPermiso,
            FechaPermiso = command.FechaPermiso
        };

        await _unitOfWork.Permissions.Add(permission);
        await _unitOfWork.CompleteAsync();
    }

    public async Task Handle(ModifyPermissionCommand command)
    {
        var permission = await _unitOfWork.Permissions.GetById(command.Id);
        if (permission != null)
        {
            permission.NombreEmpleado = command.NombreEmpleado;
            permission.ApellidoEmpleado = command.ApellidoEmpleado;
            permission.TipoPermiso = command.TipoPermiso;
            permission.FechaPermiso = command.FechaPermiso;

            _unitOfWork.Permissions.Update(permission);
            await _unitOfWork.CompleteAsync();
        }
    }
}

