using PermissionsAPI.Data;
using PermissionsAPI.Models;
using System.Security;

namespace PermissionsAPI.CQRS.Queries;

public class QueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public QueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PermissionEntity>> Handle(GetPermissionsQuery query)
    {
        return await _unitOfWork.Permissions.GetPermissionsWithTypes();
    }
}

