using MediatR;
using PermissionsAPI.Models;

namespace PermissionsAPI.CQRS.Queries;

public class GetPermissionsTypeByIdQuery : IRequest<PermissionType>
{
    public int Id { get; set; }

    public GetPermissionsTypeByIdQuery(int id)
    {
        Id = id;
    }
}
