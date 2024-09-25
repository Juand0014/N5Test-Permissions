using AutoMapper;
using MediatR;
using PermissionsAPI.Data;
using PermissionsAPI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PermissionsAPI.CQRS.Queries
{
    public class GetPermissionTypeByIdQueryHandler : IRequestHandler<GetPermissionsTypeByIdQuery, PermissionType>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPermissionTypeByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
        }
        public async Task<PermissionType> Handle(GetPermissionsTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var permissionType = await _unitOfWork.PermissionTypes.GetById(request.Id);
            return _mapper.Map<PermissionType>(permissionType);
        }
    }
}
