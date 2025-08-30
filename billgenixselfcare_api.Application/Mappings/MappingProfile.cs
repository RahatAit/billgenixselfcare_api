using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Department;
using billgenixselfcare_api.Domain.Entities;

namespace billgenixselfcare_api.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();
        }
    }
}
