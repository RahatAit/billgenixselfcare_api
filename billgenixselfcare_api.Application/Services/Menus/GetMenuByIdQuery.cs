using AutoMapper;
using billgenixselfcare_api.Application.DTOs.Menu;
using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Common;
using billgenixselfcare_api.Domain.Entities;
using MediatR;

namespace billgenixselfcare_api.Application.Services.Menus
{
    public class GetMenuByIdQuery : IRequest<Result<MenuDto>>
    {
        public int Id { get; set; }
    }

    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, Result<MenuDto>>
    {
        private readonly IRepository<Menu> _repository;
        private readonly IMapper _mapper;

        public GetMenuByIdQueryHandler(IRepository<Menu> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MenuDto>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _repository.GetByIdAsync(request.Id);
                if (data == null)
                {
                    return Result<MenuDto>.FailureResult("Not found");
                }

                var dto = _mapper.Map<MenuDto>(data);
                return Result<MenuDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return Result<MenuDto>.FailureResult("Failed to retrieve", new List<string> { ex.Message });
            }
        }
    }
}
