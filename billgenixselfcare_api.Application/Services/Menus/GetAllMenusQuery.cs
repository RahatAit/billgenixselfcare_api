using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Menu;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Services.Menus
{
    public class GetAllMenusQuery : IRequest<Result<List<MenuDto>>>
    {
    }

    public class GetAllMenusQueryHandler : IRequestHandler<GetAllMenusQuery, Result<List<MenuDto>>>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public GetAllMenusQueryHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<MenuDto>>> Handle(GetAllMenusQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.FindAsync(p => p.IsActive);
                var dto = _mapper.Map<List<MenuDto>>(data);

                return Result<List<MenuDto>>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<List<MenuDto>>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
