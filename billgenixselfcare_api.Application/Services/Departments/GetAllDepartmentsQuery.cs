using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Department;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Services.Departments
{
    public class GetAllDepartmentsQuery : IRequest<Result<List<DepartmentDto>>>
    {
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, Result<List<DepartmentDto>>>
    {
        private readonly IRepository<Department> _repository;
        private readonly IMapper _mapper;

        public GetAllDepartmentsQueryHandler(IRepository<Department> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var departments = await _repository.FindAsync(p => p.IsActive);
                var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);

                return Result<List<DepartmentDto>>.SuccessResult(departmentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<DepartmentDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
