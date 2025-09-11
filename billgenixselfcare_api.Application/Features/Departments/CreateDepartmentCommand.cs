using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Department;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Features.Departments
{
    public class CreateDepartmentCommand : IRequest<Result<DepartmentDto>>
    {
        public string Name { get; set; }
    }

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
    {
        private readonly IRepository<Department> _repository;
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IRepository<Department> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = new Department
                {
                    Name = request.Name
                };

                await _repository.AddAsync(department);
                var departmentDto = _mapper.Map<DepartmentDto>(department);

                return Result<DepartmentDto>.SuccessResult(departmentDto, "Created successfully");
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.FailureResult("Failed to create", new List<string> { ex.Message });
            }
        }
    }
}
