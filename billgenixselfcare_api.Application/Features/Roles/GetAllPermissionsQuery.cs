using AutoMapper;
using billgenixselfcare_api.Application.DTOs;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Roles
{
    public class GetAllPermissionsQuery : IRequest<Result<List<PermissionDto>>>
    {
    }

    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<List<PermissionDto>>>
    {
        private readonly IRepository<Permission> _repository;
        private readonly IMapper _mapper;

        public GetAllPermissionsQueryHandler(IRepository<Permission> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.FindAsync(r => r.IsActive);
                var dto = _mapper.Map<List<PermissionDto>>(data);
                return Result<List<PermissionDto>>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<List<PermissionDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
