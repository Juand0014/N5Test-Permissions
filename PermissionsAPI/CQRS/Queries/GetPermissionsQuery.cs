
using MediatR;
using PermissionsAPI.Models;
using System;

namespace PermissionsAPI.CQRS.Queries
{
    public class GetPermissionsQuery
    {
    }

    public class GetPermissionByIdQuery : IRequest<PermissionEntity>
    {
        public Guid Id { get; set; }

        public GetPermissionByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
