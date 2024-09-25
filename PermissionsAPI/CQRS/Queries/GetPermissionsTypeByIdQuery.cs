using MediatR;
using PermissionsAPI.Models;
using System;

namespace PermissionsAPI.CQRS.Queries;

public class GetPermissionsTypeByIdQuery : IRequest<PermissionType>
{
    public Guid Id { get; set; }

    public GetPermissionsTypeByIdQuery(Guid id)
    {
        Id = id;
    }
}
